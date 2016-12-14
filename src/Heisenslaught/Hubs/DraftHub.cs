﻿using Heisenslaught.Infrastructure;
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
        private static DraftHandler HandleDraft;

    

        public AdminDraftConfig ConfigDraft(DraftConfig cfg)
        {
            stopDraftUpdates();
            CurrentDraftConfig = new AdminDraftConfig(cfg);
            Clients.All.updateConfig(cfg);
            return CurrentDraftConfig;
        }

        public AdminDraftConfig getCurrentAdminConfig()
        {
            return CurrentDraftConfig;
        }

        public AdminDraftConfig resetDraft()
        {
            stopDraftUpdates();
            if(CurrentDraftConfig != null)
            {
                CurrentDraftConfig.reset();
                Clients.All.updateConfig(CurrentDraftConfig.getConfig());
            }
            return CurrentDraftConfig;
        }

        public AdminDraftConfig closeDraft()
        {
            stopDraftUpdates();
            if (CurrentDraftConfig != null)
            {
                DraftConfig cfg = CurrentDraftConfig.getConfig();
                cfg.state.phase = DraftStatePhase.FINISHED;
                Clients.All.updateConfig(cfg);
            }
            return CurrentDraftConfig;
        }


        public DraftConfig connectToDraft(string draftToken, string teamToken = null)
        {
            string userName = "";
            int team = 0;
            if (ConnectedUsers == null)
            {
                ConnectedUsers = new List<string>();
            }

            if (CurrentDraftConfig.draftToken != draftToken)
            {
                return null;
            }

            if(teamToken != null)
            {
                if(CurrentDraftConfig.team1DrafterToken == teamToken)
                {
                    team = 1;
                    userName = "team1Drafter-" + ConnectedUsers.Count;
            
                }
                else if ( CurrentDraftConfig.team2DrafterToken == teamToken)
                {
                    team = 2;
                    userName = "team2Drafter-" + ConnectedUsers.Count;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                userName = "observer-"  + ConnectedUsers.Count;
            }

            ConnectedUsers.Add(userName);
            //Clients.Caller.getConnectedUsers(ConnectedUsers);
            //Clients.Others.newUserAdded(userName);

          //  Clients.All.updateDraftState(CurrentDraft);
           // Send(userName, "Connected.");
            return CurrentDraftConfig.getConfig(team);
        }

        public bool setReady(string draftToken, string teamToken)
        {
            bool ready = false;

            if (CurrentDraftConfig.draftToken != draftToken)
            {
                return false;
            }
            if (CurrentDraftConfig.team1DrafterToken == teamToken)
            {
                CurrentDraftConfig.state.team1Ready = true;
                ready = true;
            }
            else if (CurrentDraftConfig.team2DrafterToken == teamToken)
            {
                CurrentDraftConfig.state.team2Ready = true;
                ready = true;
            }
            if(CurrentDraftConfig.state.team1Ready && CurrentDraftConfig.state.team2Ready)
            {
                startDraftUpdates();
            }
            Clients.All.updateDraftState(CurrentDraftConfig.state);
            return ready;
        }


        private void startDraftUpdates()
        {
            HandleDraft = new DraftHandler(CurrentDraftConfig);
            DraftUpdate = new Timer(x =>
            {
                Clients.All.updateDraftState(CurrentDraftConfig.state);
            }, null, 0, 250);
            HandleDraft.start();
        }

        private void stopDraftUpdates()
        {
            if(DraftUpdate != null)
            {
                DraftUpdate.Dispose();
                DraftUpdate = null;
            }
            if(HandleDraft != null)
            {
                HandleDraft.stop();
                HandleDraft = null;
            }
        }

        public bool pickHero(string heroId, string draftToken, string teamToken)
        {
            if (HandleDraft == null || CurrentDraftConfig == null || CurrentDraftConfig.draftToken != draftToken)
            {
                return false;
            }
            return HandleDraft.pickHero(heroId, teamToken);
        }

        public void Send(string originatorUser, string message)
        {
            Clients.All.messageReceived(originatorUser, message);
        }

        public void Pick(int team, string hero)
        {
            bool selectionSuccessful = CurrentDraft.SelectHero((Team)team, hero);
            Clients.All.updateDraftState(CurrentDraft);
            Send("Team " + team, "Selected: " + hero + (selectionSuccessful ? "" : " (ignoring)"));
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
        
                return;
            }

            ConnectedUsers.Add(newUser);
            Clients.Caller.getConnectedUsers(ConnectedUsers);
            Clients.Others.newUserAdded(newUser);
        }
    }
}
