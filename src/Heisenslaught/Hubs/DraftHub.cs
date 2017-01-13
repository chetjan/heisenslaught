using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Heisenslaught.DataTransfer;
using Heisenslaught.Exceptions;
using Heisenslaught.Services;
using Microsoft.AspNetCore.Identity;
using Heisenslaught.Models.Users;
namespace Heisenslaught
{
    public class DraftHub : Hub
    {

        private readonly IDraftService _draftService;
        private readonly UserManager<HSUser> _userManager;
        
        public DraftHub(UserManager<HSUser> userManager, IDraftService draftService)
        {
            _userManager = userManager;
            _draftService = draftService;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            _draftService.ClientDisconnected(this);
            return base.OnDisconnected(stopCalled);
        }

        public DraftConfigAdminDTO CreateDraft(CreateDraftDTO cfg)
        {
            return _draftService.CreateDraft(cfg);
        }

        public DraftConfigDTO ConnectToDraft(string draftToken, string teamToken = null)
        {
            return _draftService.ConnectToDraft(this, draftToken, teamToken);
        }

        public DraftConfigAdminDTO RestartDraft(string draftToken, string adminToken)
        {
            try
            {
                _draftService.GetDraftRoom(draftToken).ResetDraft(this, adminToken);
                return new DraftConfigAdminDTO(_draftService.GetDraftRoom(draftToken));
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
                _draftService.GetDraftRoom(draftToken).CloseDraft(this, adminToken);
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
                _draftService.GetDraftRoom(draftToken).SetReady(this, teamToken);
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
                _draftService.GetDraftRoom(draftToken).PickHero(this, heroId, teamToken);
            }
            catch (NullReferenceException)
            {
                throw new NoSuchDraftException();
            }
        }
    }
}
