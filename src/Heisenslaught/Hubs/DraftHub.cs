using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

using Heisenslaught.DataTransfer;
using Heisenslaught.Exceptions;
using Heisenslaught.Services;


namespace Heisenslaught
{
    public class DraftHub : Hub
    {
        private static DraftService Draft = new DraftService();

        
        public override Task OnDisconnected(bool stopCalled)
        {
            Draft.ClientDisconnected(this);
            return base.OnDisconnected(stopCalled);
        }

        public DraftConfigAdminDTO CreateDraft(CreateDraftDTO cfg)
        {
            return Draft.CreateDraft(cfg);
        }

        public DraftConfigDTO ConnectToDraft(string draftToken, string teamToken = null)
        {
            return Draft.ConnectToDraft(this, draftToken, teamToken);
        }

        public void RestartDraft(string draftToken, string adminToken)
        {
            try
            {
                Draft.GetDraftRoom(draftToken).ResetDraft(this, adminToken);
            }
            catch (NullReferenceException)
            {
                throw new NoSuchDraftException();
            }
        }

        public void CloseDraft(string draftToken, string adminToken)
        {
            try
            {
                Draft.GetDraftRoom(draftToken).CloseDraft(this, adminToken);
            }
            catch (NullReferenceException)
            {
                throw new NoSuchDraftException();
            }
        }

        public void SetReady(string draftToken, string teamToken)
        {
            try
            {
                Draft.GetDraftRoom(draftToken).SetReady(this, teamToken);
            }
            catch (NullReferenceException)
            {
                throw new NoSuchDraftException();
            }
        }

        public void PickHero(string heroId, string draftToken, string teamToken)
        {
            try
            {
                Draft.GetDraftRoom(draftToken).PickHero(this, heroId, teamToken);
            }
            catch (NullReferenceException)
            {
                throw new NoSuchDraftException();
            }
        }







        /*
        public AdminDraftConfig ConfigDraft(DraftConfig cfg)
        {

            var config = new CreateDraftDTO();
            config.bankTime = cfg.bankTime;
            config.bonusTime = cfg.bonusTime;
            config.disabledHeroes = cfg.disabledHeroes;
            config.firstPick = cfg.firstPick;
            config.map = cfg.map;
            config.pickTime = cfg.pickTime;
            config.team1Name = cfg.team1Name;
            config.team2Name = cfg.team2Name;

            Draft.createDraft(config);


            stopDraftUpdates();
            CurrentDraftConfig = new AdminDraftConfig(cfg);
            Clients.All.updateConfig(cfg);
            return CurrentDraftConfig;
        }

        public AdminDraftConfig getCurrentAdminConfig()
        {
            return CurrentDraftConfig;
        }

        public AdminDraftConfig resetDraft2()
        {
            stopDraftUpdates();
            if(CurrentDraftConfig != null)
            {
                CurrentDraftConfig.reset();
                Clients.All.updateConfig(CurrentDraftConfig.getConfig());
            }
            return CurrentDraftConfig;
        }

        public AdminDraftConfig closeDraft2()
        {
            stopDraftUpdates();
            if (CurrentDraftConfig != null)
            {
                DraftConfig cfg = CurrentDraftConfig.getConfig();
                cfg.state.phase = DraftStatePhase2.FINISHED;
                Clients.All.updateConfig(cfg);
            }
            return CurrentDraftConfig;
        }


        public DraftConfig connectToDraft2(string draftToken, string teamToken = null)
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

        public bool setReady2(string draftToken, string teamToken)
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

        public bool pickHero2(string heroId, string draftToken, string teamToken)
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
        */
    }
}
