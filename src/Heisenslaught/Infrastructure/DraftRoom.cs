using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Hubs;
using Heisenslaught.Models;
using Heisenslaught.Services;

namespace Heisenslaught.Infrastructure
{

    public enum DraftConnectionType
    {
        OBSERVER,
        DRAFTER,
        ADMIN
    }
    public class DraftRoomConnection
    {
        private string _id;
        private DraftConnectionType _type;


        public DraftRoomConnection(string id, DraftConnectionType type)
        {
            _id = id;
            _type = type;
        }

        public string id
        {
            get
            {
                return _id;
            }
        }

        public DraftConnectionType type
        {
            get
            {
                return _type;
            }
        }
    }

    public class DraftRoom
    {
        private DraftModel model;
        private DraftService service;

        private Dictionary<string, DraftRoomConnection> connections = new Dictionary<string, DraftRoomConnection>();


        public DraftRoom(DraftService service, DraftModel model)
        {
            this.service = service;
            this.model = model;
        }

        public DraftModel draftModel
        {
            get
            {
                return model;
            }
        }

        public DraftRoomConnection connect(HubCallerContext callerContext, string authToken)
        {
            var connection = new DraftRoomConnection(callerContext.ConnectionId, getConnectionType(authToken));
            if (connections.ContainsKey(callerContext.ConnectionId))
                connections.Remove(callerContext.ConnectionId);
            connections.Add(callerContext.ConnectionId, connection); 
            return connection;
        }

        public bool disconnect(HubCallerContext callerContext)
        {
            return connections.Remove(callerContext.ConnectionId);
        }

        public int connectionCount
        {
            get
            {
                return connections.Count;
            }
        }

        public bool isActive
        {
            get
            {
                return (draftModel.state.phase == DraftStatePhase.PICKING) ||  connectionCount > 0;
            }
        }

        public void Dispose()
        {

        }

        private DraftConnectionType getConnectionType(string authToken)
        {
            if (authToken == draftModel.adminToken)
                return DraftConnectionType.ADMIN;
            if (authToken == draftModel.team1DrafterToken || authToken == draftModel.team2DrafterToken)
                return DraftConnectionType.DRAFTER;
            return DraftConnectionType.OBSERVER;
        }

        

    }
}
