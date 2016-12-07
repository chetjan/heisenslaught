using Heisenslaught.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Heisenslaught
{
    public class DraftHub : Hub
    {
        public static List<string> ConnectedUsers;
        public static Draft CurrentDraft;
        private static Timer DraftUpdate;

        public void Send(string originatorUser, string message)
        {
            Clients.All.messageReceived(originatorUser, CurrentDraft.CurrentAction + message);
            CurrentDraft.NextState();
            Clients.All.updateDraftState(CurrentDraft);
        }

        public void Connect(string newUser)
        {
            if (ConnectedUsers == null)
            {
                ConnectedUsers = new List<string>();
            }

            if (CurrentDraft == null)
            {
                CurrentDraft = new Draft();
            }

            if (newUser == "reset")
            {
                CurrentDraft = new Draft();
                Send("Admin", "Resetting draft...");
                return;
            }

            if (newUser == "start")
            {
                CurrentDraft = new Draft();
                DraftUpdate = new Timer(x =>
                {
                    Clients.All.updateDraftState(CurrentDraft);
                }, null, 0, 1);
                Send("Admin", "Starting draft...");
                return;
            }

            ConnectedUsers.Add(newUser);
            Clients.Caller.getConnectedUsers(ConnectedUsers);
            Clients.Others.newUserAdded(newUser);
        }
    }
}
