using Heisenslaught.DataTransfer;
using Heisenslaught.Exceptions;
using Heisenslaught.Hubs;
using Heisenslaught.Models.Users;
using Heisenslaught.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using System;
using System.Threading.Tasks;


namespace Heisenslaught
{
    public class DraftHub : UserAwareHub
    {

        private readonly IDraftService _draftService;
    
        public DraftHub(IDraftService draftService, IHubConnectionsService connectionService, UserManager<HSUser> userManager) : base(connectionService, userManager)
        {
            _draftService = draftService;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            _draftService.ClientDisconnected(this);
            return base.OnDisconnected(stopCalled);
        }

        [HubMethodName("createDraft")]
        public async Task<DraftConfigAdminDTO> CreateDraftAsync(CreateDraftDTO cfg)
        {
            return await _draftService.CreateDraftAsync(cfg, this);
        }

        [HubMethodName("connectToDraft")]
        public async Task<DraftConfigDTO> ConnectToDraftAsync(string draftToken, string teamToken = null)
        {
            return await _draftService.ConnectToDraftAsync(this, draftToken, teamToken);
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
