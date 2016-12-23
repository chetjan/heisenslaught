using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heisenslaught.Infrastructure;
using Heisenslaught.Models;
using Heisenslaught.DataTransfer;
using Microsoft.AspNetCore.SignalR.Hubs;
using Heisenslaught.Persistence;


namespace Heisenslaught.Services
{
    public class DraftService
    {
        private static MongoDraftRepository draftRepo = new MongoDraftRepository();
        private Dictionary<string, DraftRoom> activeRooms = new Dictionary<string, DraftRoom>();
        private Dictionary<string, DraftRoom> connectionsRoom = new Dictionary<string, DraftRoom>();


        public DraftConfigAdminDTO CreateDraft(CreateDraftDTO config)
        {
            var model = new DraftModel(config.ToModel());
            draftRepo.CreateDraft(model);
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
                DraftModel config = draftRepo.FindByDraftToken(draftToken);
                room = new DraftRoom(this, config);
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
            draftRepo.SaveDraft(room.DraftModel);
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

        /*
        public List<DraftConfigAdminDTO> getActiveDrafts()
        {
            List<DraftConfigAdminDTO> list = new List<DraftConfigAdminDTO>();
            foreach (var pair in activeRooms)
            {
                list.Add(new DraftConfigAdminDTO(pair.Value.DraftModel));
            }
            return list;
        }

        public List<DraftConfigAdminDTO> getDrafts(int start = 0, int limit = 0)
        {
            return null;
        }

        public DraftConfigDTO getDraftConfig(string draftToken, string authToken = null)
        {
            DraftModel model = null;
            var room = activeRooms[draftToken];
            if(room != null)
            {
                model = room.DraftModel;
            }
            else
            {
                model = draftRepo.findByDraftToken(draftToken);
            }
            if (model != null)
            {
                if(model.adminToken == authToken)
                {
                    return new DraftConfigAdminDTO(model);
                }
                else if(model.team1DrafterToken == authToken || model.team2DrafterToken == authToken)
                {
                    return new DraftConfigDrafterDTO(model, authToken);
                }
                else
                {
                    return new DraftConfigDTO(model);
                }
            }
            return null;
        }

    */
    }
}
