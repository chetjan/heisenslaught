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

        private static AdminDraftConfig CurrentDraftConfig;

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

        public AdminDraftConfig ConfigDraft(DraftConfig cfg)
        {
            CurrentDraftConfig = new AdminDraftConfig(cfg);
    
            CurrentDraft = new Draft();
            Clients.All.updateConfig(cfg);

            return CurrentDraftConfig;
        }

        public AdminDraftConfig getCurrentAdminConfig()
        {
            return CurrentDraftConfig;

        }

        public AdminDraftConfig resetDraft()
        {
            if(CurrentDraftConfig != null)
            {
                CurrentDraftConfig.reset();
                Clients.All.updateConfig(CurrentDraftConfig.getConfig());
            }
            return CurrentDraftConfig;
        }

        public AdminDraftConfig closeDraft()
        {
            if (CurrentDraftConfig != null)
            {
                DraftConfig cfg = CurrentDraftConfig.getConfig();
                cfg.state.phase = DraftStatePhase.FINISHED;
                Clients.All.updateConfig(cfg);
            }
            return CurrentDraftConfig;
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
