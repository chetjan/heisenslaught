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

        public DraftConfigAdminDTO RestartDraft(string draftToken, string adminToken)
        {
            try
            {
                Draft.GetDraftRoom(draftToken).ResetDraft(this, adminToken);
                return new DraftConfigAdminDTO(Draft.GetDraftRoom(draftToken));
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
    }
}
