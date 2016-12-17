using System.Collections.Generic;
using Heisenslaught.Models;
using Heisenslaught.Services;
using Heisenslaught.Exceptions;
using Heisenslaught.DataTransfer;

namespace Heisenslaught.Infrastructure
{
    public class DraftRoom
    {
        private Dictionary<string, DraftRoomConnection> connections = new Dictionary<string, DraftRoomConnection>();
        private DraftModel model;
        private string roomName;
        private DraftService service;
        private DraftHandler draftHandler;

        public DraftRoom(DraftService service, DraftModel model)
        {
            this.service = service;
            this.model = model;
            this.roomName = "draftRoom-" + model.draftToken;
            this.draftHandler = new DraftHandler(this);
        }

        public DraftModel DraftModel
        {
            get
            {
                return model;
            }
        }

        public DraftRoomConnection Connect(DraftHub hub, string authToken)
        {
            var connection = new DraftRoomConnection(hub.Context.ConnectionId, GetConnectionType(authToken));
            if (!connections.ContainsKey(hub.Context.ConnectionId))
            {
                connections.Add(hub.Context.ConnectionId, connection);
                hub.Groups.Add(hub.Context.ConnectionId, roomName);
            }
            return connection;
        }

        public bool Disconnect(DraftHub hub)
        {
            return connections.Remove(hub.Context.ConnectionId);
        }

        public int ConnectionCount
        {
            get
            {
                return connections.Count;
            }
        }

        public bool IsActive
        {
            get
            {
                return (DraftModel.state.phase == DraftStatePhase.PICKING) ||  ConnectionCount > 0;
            }
        }

        private DraftStateModel DraftState
        {
            get
            {
                return DraftModel.state;
            }
        }

        private DraftStatePhase Phase
        {
            get
            {
                return DraftState.phase;
            }
        }

        public void Dispose()
        {

        }

        private DraftConnectionType GetConnectionType(string authToken)
        {
            if (authToken == DraftModel.adminToken)
                return DraftConnectionType.ADMIN;
            if (authToken == DraftModel.team1DrafterToken || authToken == DraftModel.team2DrafterToken)
                return DraftConnectionType.DRAFTER;
            return DraftConnectionType.OBSERVER;
        }

        public void ResetDraft(DraftHub hub, string authToken)
        {
            if(Phase == DraftStatePhase.FINISHED)
            {
                throw new MethodNotAllowedInPhaseException();
            }
            var conType = GetConnectionType(authToken);
            if(conType != DraftConnectionType.ADMIN)
            {
                throw new InsufficientDraftPermissionsException();
            }

            DraftModel.state = new DraftStateModel();
            UpdateDraftState(hub);
        }

        public void CloseDraft(DraftHub hub, string authToken)
        {
            if (Phase == DraftStatePhase.FINISHED)
            {
                throw new MethodNotAllowedInPhaseException();
            }
            var conType = GetConnectionType(authToken);
            if (conType != DraftConnectionType.ADMIN)
            {
                throw new InsufficientDraftPermissionsException();
            }
            DraftState.phase = DraftStatePhase.FINISHED;
            UpdateDraftState(hub);
        }

        public void SetReady(DraftHub hub, string authToken)
        {
            if (Phase != DraftStatePhase.WAITING)
            {
                throw new MethodNotAllowedInPhaseException();
            }
            var conType = GetConnectionType(authToken);
            if (conType != DraftConnectionType.DRAFTER)
            {
                throw new InsufficientDraftPermissionsException();
            }
            if(DraftModel.team1DrafterToken == authToken)
            {
                DraftState.team1Ready = true;
            }
            else if (DraftModel.team2DrafterToken == authToken)
            {
                DraftState.team2Ready = true;
            }
            // TODO impl draft handler
            UpdateDraftState(hub);
        }

        public void PickHero(DraftHub hub, string heroId, string authToken)
        {
            if (Phase != DraftStatePhase.PICKING)
            {
                throw new MethodNotAllowedInPhaseException();
            }
            var conType = GetConnectionType(authToken);
            if (conType != DraftConnectionType.DRAFTER)
            {
                throw new InsufficientDraftPermissionsException();
            }
            // TODO: pick hero
            UpdateDraftState(hub);
        }


        private void UpdateDraftState(DraftHub hub)
        {
            hub.Clients.Group(roomName).updateDraftState(new DraftStateDTO(DraftState));
        }

    }

    public enum DraftConnectionType
    {
        OBSERVER,
        DRAFTER,
        ADMIN
    }

    public class DraftRoomConnection
    {
        private string id;
        private DraftConnectionType type;


        public DraftRoomConnection(string id, DraftConnectionType type)
        {
            this.id = id;
        
            this.type = type;
        }

        public string Id
        {
            get
            {
                return id;
            }
        }

        public DraftConnectionType Type
        {
            get
            {
                return type;
            }
        }
    }
}
