using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using System.Threading;
using Heisenslaught.Models.Users;
using Microsoft.AspNetCore.SignalR;

namespace Heisenslaught.Services
{
    public class HubConnection{
        public Type HubType;
        public string ConnectionId;
        public HSUser User; 

        public HubConnection(HSUser user, Hub hub)
        {
            HubType = hub.GetType();
            ConnectionId = hub.Context.ConnectionId;
            User = user;
        }
    }

    public class HubChannelConnection
    {
        public HubConnection Connection;
        public string ChannelName;

        public HubChannelConnection(HubConnection connection, string channelName)
        {
            Connection = connection;
            ChannelName = channelName;
        }
    }
    
    public class HubConnectionsService : IHubConnectionsService
    {

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private readonly Dictionary<string, HSUser> _connectedUsers = new Dictionary<string, HSUser>();
        private readonly List<HubConnection> _hubConnections = new List<HubConnection>();
        private readonly List<HubChannelConnection> _channelConnections = new List<HubChannelConnection>();

        private HubConnection FindHubConnection(Hub hub)
        {
            return (from c in _hubConnections
                    where c.HubType == hub.GetType() && c.ConnectionId == hub.Context.ConnectionId
                    select c).FirstOrDefault();
        }

        private IEnumerable<HubConnection> FindHubConnectionByUserId(string userId)
        {
            return from c in _hubConnections
                   where c.User.Id == userId
                   select c;
        }

        private IEnumerable<HubChannelConnection> FindChannelConnectionsByUserId(string userId)
        {
            return from c in _channelConnections
                   where c.Connection.User.Id == userId
                   select c;
        }

        private HubChannelConnection FindChannelConnection(Hub hub, string channelName)
        {
            return (from c in _channelConnections
                    where c.ChannelName == channelName && c.Connection.HubType == hub.GetType() && c.Connection.ConnectionId == hub.Context.ConnectionId
                    select c).FirstOrDefault();
        }

        private HSUser GetConnectedUser(string userId)
        {
            if (IsUserConnected(userId))
                return _connectedUsers[userId];
            return null;
        }

        public HubConnection OnUserConnected(HSUser user, Hub hub)
        {
            _lock.EnterWriteLock();
            try
            {

                var hubConnection = FindHubConnection(hub);
                if (hubConnection == null)
                {
                    var u = GetConnectedUser(user.Id);
                    if (u == null)
                    {
                        u = user;
                        _connectedUsers.Add(u.Id, u);
                    }
                    hubConnection = new HubConnection(u, hub);
                    _hubConnections.Add(hubConnection);
                }
                return hubConnection;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void OnUserDisconnected(HSUser user, Hub hub)
        {
            _lock.EnterWriteLock();
            try
            {
                var hubConnection = FindHubConnection(hub);
                if (hubConnection != null)
                {
                    if (_hubConnections.Remove(hubConnection))
                    {
                        var userHubConnections = FindHubConnectionByUserId(user.Id);
                        if (userHubConnections.Count() == 0)
                        {
                            _connectedUsers.Remove(user.Id);
                            // remove channel connections
                            var channels = FindChannelConnectionsByUserId(user.Id);
                            for (var i = 0; i < channels.Count(); i++)
                            {
                                var channel = channels.ElementAt(i);
                                _channelConnections.Remove(channel);
                            }
                        }
                    }
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void OnUserJoinedChannel(HSUser user, Hub hub, string channelName)
        {
            _lock.EnterWriteLock();
            try
            {
                var channelConnection = FindChannelConnection(hub, channelName);
                if (channelConnection == null)
                {
                    var connection = FindHubConnection(hub);
                    if (connection == null)
                    {
                        connection = OnUserConnected(user, hub);
                    }
                    channelConnection = new HubChannelConnection(connection, channelName);
                    _channelConnections.Add(channelConnection);

                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void OnUserLeftChannel(Hub hub, string channelName)
        {
            _lock.EnterWriteLock();
            try
            {
                var channelConnection = FindChannelConnection(hub, channelName);
                if (channelConnection != null)
                {
                    _channelConnections.Remove(channelConnection);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }



        public bool IsUserConnected(string userId)
        {
            try
            {
                _lock.EnterReadLock();
                return _connectedUsers.ContainsKey(userId);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

     

        public bool IsUserConnected(string userId, Type hubType)
        {
            try
            {
                _lock.EnterReadLock();
                return (from c in _hubConnections
                        where c.HubType == hubType && c.User.Id == userId
                        select c).Any();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public bool IsUserConnected(string userId, Type hubType, string channelName)
        {
            try
            {
                _lock.EnterReadLock();
                return (from c in _channelConnections
                        where c.Connection.HubType == hubType && c.Connection.User.Id == userId && c.ChannelName == channelName
                        select c).Any();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
      
        public List<HSUser> GetConnectedUsers()
        {
            try
            {
                _lock.EnterReadLock();
                return _connectedUsers.Values.Distinct().ToList<HSUser>();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public List<HSUser> GetConnectedUsers(Type hubType)
        {
            try
            {
                _lock.EnterReadLock();
                return (from c in _hubConnections
                        where c.HubType == hubType
                        select c.User).Distinct().ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public List<HSUser> GetConnectedUsers(Type hubType, string channelName)
        {
            try
            {
                _lock.EnterReadLock();
                return (from c in _channelConnections
                        where c.Connection.HubType == hubType && c.ChannelName == channelName
                        select c.Connection.User).Distinct().ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public List<Type> GetUserHubs(string userId)
        {
            try
            {
                _lock.EnterReadLock();
                return (from c in _hubConnections
                        where c.User.Id == userId
                        select c.HubType).Distinct().ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public List<string> GetUserChannels(string userId, Type hubType)
        {
            try
            {
                _lock.EnterReadLock();
                return (from c in _channelConnections
                        where c.Connection.User.Id == userId && c.Connection.HubType == hubType
                        select c.ChannelName).Distinct().ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

    }
}
