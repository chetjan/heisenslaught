using Heisenslaught.DataTransfer;
using Heisenslaught.Exceptions;
using Heisenslaught.Models.Draft;
using Heisenslaught.Models.Users;
using Heisenslaught.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Heisenslaught.Infrastructure
{
    public class DraftRoom
    {
        private readonly IHeroDataService _heroDataService;
        private readonly IHubConnectionsService _conService;

        private DraftHandler draftHandler;
        private DraftModel model;
        private DraftService service;
        private Timer timer;


        public DraftRoom(DraftService service, IHeroDataService heroDataService, IHubConnectionsService conService, DraftModel model)
        {
            this.service = service;
            _conService = conService;
            _heroDataService = heroDataService;
            this.model = model;
            this.RoomName = "draftRoom-" + model.draftToken;
            this.draftHandler = new DraftHandler(this, _heroDataService);
        }

        public string RoomName { get; private set; }

        public DraftModel DraftModel
        {
            get
            {
                return model;
            }
        }

        public DraftConnectionType Connect(DraftHub hub, HSUser user, string authToken)
        {
            if (user != null)
            {
                var connectionType = GetConnectionType(authToken);
                hub.Groups.Add(hub.Context.ConnectionId, RoomName);
                _conService.OnUserJoinedChannel(user, hub, RoomName, (int)connectionType);

                hub.Clients.Caller.SetConnectedUsers(GetDraftConnections());
                hub.Clients.Group(RoomName, new string[] { hub.Context.ConnectionId }).OnUserJoined(GetDraftConnection(user.Id));
                return connectionType;
            }
            return DraftConnectionType.NONE;
        }

        public void Disconnect(DraftHub hub)
        {
            var user = _conService.GetUserFromConnection(hub.Context.ConnectionId);
            if (user != null) {
                var connection = GetDraftConnection(user.Id);
                _conService.OnUserLeftChannel(hub, RoomName);
                if(user == null || !_conService.IsUserConnected(user.Id, hub.GetType(), RoomName))
                {
                    // send user left message
                    hub.Clients.Group(RoomName, new string[] { hub.Context.ConnectionId }).OnUserLeft(connection);
                }
                else
                {
                    hub.Clients.Group(RoomName, new string[] { hub.Context.ConnectionId }).OnUserStatusUpdate(GetDraftConnection(user.Id));
                }
            }
        }

        protected IEnumerable<DraftRoomConnection> _getDraftConnections(string userId = null)
        {
            var result =_conService.Query<IEnumerable<DraftRoomConnection>>((List<HubChannelConnection> cons) => {
                var q = from c in cons
                        where c.ChannelName == RoomName && (userId == null || c.Connection.User.Id == userId)
                        group c by c.Connection.User.Id into usrGroup

                        select new DraftRoomConnection {
                            id = usrGroup.FirstOrDefault().Connection.User.Id,
                            name = usrGroup.FirstOrDefault().Connection.User.BattleTagDisplay,
                            connectionTypes = usrGroup.Select(t=> t.Flag).Distinct().Sum()
                        };
                return q;
            });
            return result;
        }

        protected DraftRoomConnection GetDraftConnection(string userId)
        {
            return _getDraftConnections(userId).FirstOrDefault();
        }

        protected List<DraftRoomConnection> GetDraftConnections()
        {
            return _getDraftConnections().ToList();
        }

        public int ConnectionCount
        {
            get
            {
                return _conService.GetConnectedUsers(typeof(DraftHub), RoomName).Count;
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
            StopTimer();
        }

        private DraftConnectionType GetConnectionType(string authToken)
        {
            if (authToken == DraftModel.adminToken)
                return DraftConnectionType.ADMIN;
            if (authToken == DraftModel.team1DrafterToken)
                return DraftConnectionType.DRAFTER_TEAM_1;
            if (authToken == DraftModel.team2DrafterToken)
                return DraftConnectionType.DRAFTER_TEAM_2;
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
            StopTimer();
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
            StopTimer();
            CompleteDraft();
        }

        public void SetReady(DraftHub hub, string authToken)
        {
            if (Phase != DraftStatePhase.WAITING)
            {
                throw new MethodNotAllowedInPhaseException();
            }
            var conType = GetConnectionType(authToken);
            if (conType != DraftConnectionType.DRAFTER_TEAM_1 && conType != DraftConnectionType.DRAFTER_TEAM_2)
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
            if(DraftState.team1Ready && DraftState.team2Ready)
            {
                StartTimer(hub);
            }
            UpdateDraftState(hub);
        }

        public void PickHero(DraftHub hub, string heroId, string authToken)
        {
            if (Phase != DraftStatePhase.PICKING)
            {
                throw new MethodNotAllowedInPhaseException();
            }
            var conType = GetConnectionType(authToken);
            if (conType != DraftConnectionType.DRAFTER_TEAM_1 && conType != DraftConnectionType.DRAFTER_TEAM_2)
            {
                throw new InsufficientDraftPermissionsException();
            }
            draftHandler.PickHero(heroId, authToken);
            if (DraftState.phase == DraftStatePhase.FINISHED)
            {
                StopTimer();
                CompleteDraft();
            }
            UpdateDraftState(hub);
        }

        private void UpdateDraftState(DraftHub hub)
        {
            hub.Clients.Group(RoomName).updateDraftState(new DraftStateDTO(this));
        }

        public void StartTimer(DraftHub hub)
        {
            if (timer == null)
            {
                timer = new Timer(x=> {
                    if (draftHandler.Tick())
                    {
                        UpdateDraftState(hub);
                        if(DraftState.phase == DraftStatePhase.FINISHED)
                        {
                            StopTimer();
                            CompleteDraft();
                        }
                    }
                }, null, 0, 1000);
            }

        }

        public void StopTimer()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

        private void CompleteDraft()
        {
            service.CompleteDraft(this);
        }
    }

    [Flags]
    public enum DraftConnectionType
    {
        NONE = 0,
        DRAFTER_TEAM_1 = 1,
        DRAFTER_TEAM_2 = 2,
        OBSERVER = 4,
        ADMIN = 8
    }

    public class DraftRoomConnection
    {
        public string id;
        public string name;
        public int connectionTypes;
    }
}
