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
        private static MongoDraftRepository DraftRepo = new MongoDraftRepository();

        private Dictionary<string, DraftRoom> activeRooms = new Dictionary<string, DraftRoom>();
        private Dictionary<string, DraftRoom> connectionsRoom = new Dictionary<string, DraftRoom>();
//        private Dictionary<DraftRoom, List<string>> roomConnections = new Dictionary<DraftRoom, List<string>>();

        public DraftHub hub;

        public DraftService(DraftHub hub)
        {
            this.hub = hub;
        }

        public DraftConfigAdminDTO createDraft(CreateDraftDTO config)
        {
            var model = new DraftModel(config.ToModel());
            DraftRepo.createDraft(model);
            return new DraftConfigAdminDTO(model);
        }


        public DraftConfigDTO connectToDraft(HubCallerContext callerContext, string draftToken, string authToken = null)
        {
            DraftConfigDTO config = null;
            var room = getDraftRoom(draftToken, true);
            var connection = room.connect(callerContext, authToken);
            // TODO: if user is connecected to another room disconnect from that room or support multi room connections;
            if(!connectionsRoom.ContainsKey(callerContext.ConnectionId))
                connectionsRoom.Add(callerContext.ConnectionId, room);

            tryActivateDraftRoom(room);

            switch (connection.type)
            {
                case DraftConnectionType.ADMIN:
                    config = new DraftConfigAdminDTO(room.draftModel);
                    break;
                case DraftConnectionType.DRAFTER:
                    config = new DraftConfigDrafterDTO(room.draftModel, authToken);
                    break;
                case DraftConnectionType.OBSERVER:
                    config = new DraftConfigDTO(room.draftModel);
                    break;
            }
               
            return config;
        }


        public DraftRoom getDraftRoom(string draftToken, bool autoCreate=false)
        {
            var room = activeRooms.ContainsKey(draftToken) ? activeRooms[draftToken] : null;
            if (room == null && autoCreate)
            {
                DraftModel config = DraftRepo.findByDraftToken(draftToken);
                room = new DraftRoom(this, config);
            }
            return room;
        }


        public void clientDisconnected(HubCallerContext callerContext)
        {
            var id = callerContext.ConnectionId;
            if (connectionsRoom.ContainsKey(id))
            {
                var room = connectionsRoom[id];
                if (room.disconnect(callerContext))
                {
                    connectionsRoom.Remove(id);
                    tryDeactivateDraftRoom(room);
                }
            }
        }


        private void tryActivateDraftRoom(DraftRoom room)
        {
            if(!activeRooms.ContainsKey(room.draftModel.draftToken) && room.isActive)
            {
                activeRooms.Add(room.draftModel.draftToken, room);
            }
        }

        private void tryDeactivateDraftRoom(DraftRoom room)
        {
            if (activeRooms.ContainsKey(room.draftModel.draftToken) && !room.isActive)
            {
                activeRooms.Remove(room.draftModel.draftToken);
                room.Dispose();
            }
        }












        public List<DraftConfigAdminDTO> getActiveDrafts()
        {
            List<DraftConfigAdminDTO> list = new List<DraftConfigAdminDTO>();
            foreach (var pair in activeRooms)
            {
                list.Add(new DraftConfigAdminDTO(pair.Value.draftModel));
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
                model = room.draftModel;
            }
            else
            {
                model = DraftRepo.findByDraftToken(draftToken);
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

       

        

        

        public DraftConfigAdminDTO resetDraft(string draftToken, string admintoken)
        {
            return null;
        }

        public bool closeDraft(string draftToken, string admintoken)
        {
            return false;
        }

        public bool setReady(string draftToken, string teamToken)
        {
            return false;
        }

        public bool pickHero(string heroId, string draftToken, string teamToken)
        {
            return false;
        }
    }
}
