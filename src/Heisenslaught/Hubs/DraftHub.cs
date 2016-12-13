using Heisenslaught.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Heisenslaught
{
    public class DraftHub : Hub
    {
        public static List<string> ConnectedUsers;
        public static Draft CurrentDraft;
        private static Timer DraftUpdate;

        public void Send(string originatorUser, string message)
        {
            Clients.All.messageReceived(originatorUser, message);
        }

        public void Pick(int team, string hero)
        {
            bool selectionSuccessful = CurrentDraft.SelectHero((Team) team, hero);
            Clients.All.updateDraftState(CurrentDraft);
            Send("Team " + team, "Selected: " + hero + (selectionSuccessful ? "" : " (ignoring)"));
        }

        public DraftConfig ConfigDraft(DraftConfig cfg)
        {
            //DraftConfig cfg = JsonConvert.DeserializeObject<DraftConfig>(config);
            CurrentDraft = new Draft();
            Clients.All.updateConfig(cfg);

            return cfg;
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
                CurrentDraft.StartDraft();
                return;
            }

            ConnectedUsers.Add(newUser);
            Clients.Caller.getConnectedUsers(ConnectedUsers);
            Clients.Others.newUserAdded(newUser);
        }
    }
}
