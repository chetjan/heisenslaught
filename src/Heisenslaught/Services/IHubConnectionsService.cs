using System;
using System.Collections.Generic;
using Heisenslaught.Models.Users;
using Microsoft.AspNetCore.SignalR;

namespace Heisenslaught.Services
{
    public interface IHubConnectionsService
    {
        List<HSUser> GetConnectedUsers();
        List<HSUser> GetConnectedUsers(Type hubType);
        List<HSUser> GetConnectedUsers(Type hubType, string channelName);
        HSUser GetUserFromConnection(string connectionId);
        List<string> GetUserChannels(string userId, Type hubType);
        List<Type> GetUserHubs(string userId);
        bool IsUserConnected(string userId);
        bool IsUserConnected(string userId, Type hubType);
        bool IsUserConnected(string userId, Type hubType, string channelName);
        HubConnection OnUserConnected(HSUser user, Hub hub);
        void OnUserDisconnected(HSUser user, Hub hub);
        void OnUserJoinedChannel(HSUser user, Hub hub, string channelName, int flag = 0);
        void OnUserLeftChannel(Hub hub, string channelName);
        TReturn Query<TReturn>(Func<Dictionary<string, HSUser>, TReturn> query);
        TReturn Query<TReturn>(Func<List<HubConnection>, TReturn> query);
        TReturn Query<TReturn>(Func<List<HubChannelConnection>, TReturn> query);
    }
}