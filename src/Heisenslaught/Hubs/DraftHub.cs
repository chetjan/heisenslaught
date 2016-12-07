using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heisenslaught
{
    public class DraftHub : Hub
    {
        public static List<string> ConnectedUsers;

        public void Send(string originatorUser, string message)
        {
            Clients.All.messageReceived(originatorUser, message);
        }

        public void Connect(string newUser)
        {
            if (ConnectedUsers == null)
            {
                ConnectedUsers = new List<string>();
            }

            ConnectedUsers.Add(newUser);
            Clients.Caller.getConnectedUsers(ConnectedUsers);
            Clients.Others.newUserAdded(newUser);
        }
    }
}
