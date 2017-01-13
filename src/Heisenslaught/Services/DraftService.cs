using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heisenslaught.Infrastructure;
using Heisenslaught.Models;
using Heisenslaught.DataTransfer;
using Microsoft.AspNetCore.SignalR.Hubs;
using Heisenslaught.Persistence.Draft;


namespace Heisenslaught.Services
{
    public class DraftService : IDraftService
    {
        private readonly IDraftStore _draftStore;
        private readonly HeroDataService _heroDataService;
        private Dictionary<string, DraftRoom> activeRooms = new Dictionary<string, DraftRoom>();
        private Dictionary<string, DraftRoom> connectionsRoom = new Dictionary<string, DraftRoom>();

        public DraftService(IDraftStore draftStore, HeroDataService heroDataService)
        {
            _draftStore = draftStore;
            _heroDataService = heroDataService;
        }

        public DraftConfigAdminDTO CreateDraft(CreateDraftDTO config)
        {
            var model = new DraftModel(config.ToModel());
            _draftStore.CreateDraft(model);
            return new DraftConfigAdminDTO(model);
        }

        public DraftConfigDTO ConnectToDraft(DraftHub hub, string draftToken, string authToken = null)
        {
            DraftConfigDTO config = null;
            var room = GetDraftRoom(draftToken, true);
            var connection = room.Connect(hub, authToken);
            // TODO: if user is connecected to another room disconnect from that room or support multi room connections;
            lock (connectionsRoom)
            {
                if (!connectionsRoom.ContainsKey(hub.Context.ConnectionId))
                    connectionsRoom.Add(hub.Context.ConnectionId, room);
            }

            TryActivateDraftRoom(room);

            switch (connection.Type)
            {
                case DraftConnectionType.ADMIN:
                    config = new DraftConfigAdminDTO(room);
                    break;
                case DraftConnectionType.DRAFTER:
                    config = new DraftConfigDrafterDTO(room, authToken);
                    break;
                case DraftConnectionType.OBSERVER:
                    config = new DraftConfigDTO(room);
                    break;
            }
            return config;
        }

        public DraftRoom GetDraftRoom(string draftToken, bool autoCreate=false)
        {
            var room = activeRooms.ContainsKey(draftToken) ? activeRooms[draftToken] : null;
            if (room == null && autoCreate)
            {
                DraftModel config = _draftStore.FindByDraftToken(draftToken);
                room = new DraftRoom(this, _heroDataService, config);
            }
            return room;
        }

        public void ClientDisconnected(DraftHub hub)
        {
            var id = hub.Context.ConnectionId;
            lock (connectionsRoom)
            {
                if (connectionsRoom.ContainsKey(id))
                {
                    var room = connectionsRoom[id];
                    if (room.Disconnect(hub))
                    {
                        connectionsRoom.Remove(id);
                        TryDeactivateDraftRoom(room);
                    }
                }
            }
        }

        public void CompleteDraft(DraftRoom room)
        {
            _draftStore.SaveDraft(room.DraftModel);
            TryDeactivateDraftRoom(room);
        }


        private void TryActivateDraftRoom(DraftRoom room)
        {
            lock (activeRooms)
            {
                if(!activeRooms.ContainsKey(room.DraftModel.draftToken) && room.IsActive)
                {
               
                    activeRooms.Add(room.DraftModel.draftToken, room);
                
                }
            }
        }

        private void TryDeactivateDraftRoom(DraftRoom room)
        {
            lock (activeRooms)
            {
                if (activeRooms.ContainsKey(room.DraftModel.draftToken) && !room.IsActive)
                {
                    activeRooms.Remove(room.DraftModel.draftToken);
                    room.Dispose();
                }
            }
        }

    }
}
