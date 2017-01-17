﻿using Heisenslaught.DataTransfer;
using Heisenslaught.Infrastructure;
using Heisenslaught.Models.Draft;
using Heisenslaught.Models.Users;
using Heisenslaught.Persistence.Draft;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Heisenslaught.Services
{
    public class DraftService : IDraftService
    {
        private readonly IDraftStore _draftStore;
        private readonly IHeroDataService _heroDataService;
        private readonly UserManager<HSUser> _userManager;
        private readonly IHubConnectionsService _connectionService;

        private Dictionary<string, DraftRoom> activeRooms = new Dictionary<string, DraftRoom>();
        private Dictionary<string, DraftRoom> connectionsRoom = new Dictionary<string, DraftRoom>();


        public DraftService(IDraftStore draftStore, IHeroDataService heroDataService, UserManager<HSUser> userManager, IHubConnectionsService connectionService)
        {
            _draftStore = draftStore;
            _heroDataService = heroDataService;
            _userManager = userManager;
            _connectionService = connectionService;
        }

        public async Task<DraftConfigAdminDTO> CreateDraftAsync(CreateDraftDTO config, DraftHub hub)
        {
            var model = new DraftModel(config.ToModel());
            var user = await _userManager.GetUserAsync((ClaimsPrincipal)hub.Context.User);
            model.createdBy = user.Id;
            _draftStore.Create(model);
            return new DraftConfigAdminDTO(model);
        }

        public async Task<DraftConfigDTO> ConnectToDraftAsync(DraftHub hub, string draftToken, string authToken = null)
        {
            DraftConfigDTO config = null;
            var room = GetDraftRoom(draftToken, true);
            lock (connectionsRoom)
            {
                if (!connectionsRoom.ContainsKey(hub.Context.ConnectionId))
                {
                    connectionsRoom.Add(hub.Context.ConnectionId, room);
                }

            }
           
            var user = await _userManager.GetUserAsync((ClaimsPrincipal)hub.Context.User);
            var connectionType = room.Connect(hub, user, authToken);

            TryActivateDraftRoom(room);
            

            switch (connectionType)
            {
                case DraftConnectionType.ADMIN:
                    config = new DraftConfigAdminDTO(room);
                    break;
                case DraftConnectionType.DRAFTER_TEAM_1:
                case DraftConnectionType.DRAFTER_TEAM_2:
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
                room = new DraftRoom(this, _heroDataService, _connectionService, config);
            }
            return room;
        }

        public void ClientDisconnected(DraftHub hub)
        {
            DraftRoom room = null;

            lock (connectionsRoom)
            {
                if (connectionsRoom.ContainsKey(hub.Context.ConnectionId))
                {
                    room = connectionsRoom[hub.Context.ConnectionId];
                }
            }
           
            if(room != null)
            {
                room.Disconnect(hub);
                TryDeactivateDraftRoom(room);
            }
        }

        public void CompleteDraft(DraftRoom room)
        {
            _draftStore.Update(room.DraftModel);
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
