using Heisenslaught.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Heisenslaught.Infrastructure.Hubs
{
    public abstract class UserAwareHub : Hub
    {
        protected readonly IHubConnectionsService connectionService;
        protected readonly UserManager<HSUser> userManager;

        public UserAwareHub(IHubConnectionsService connectionService, UserManager<HSUser> userManager) : base()
        {
            this.connectionService = connectionService;
            this.userManager = userManager;
        }

        public override Task OnConnected()
        {
            #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            OnUserConnectedAsync();
            #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            return base.OnConnected();
        }

        protected async Task<bool> OnUserConnectedAsync()
        {
            var user = await userManager.GetUserAsync((ClaimsPrincipal)Context.User);
            connectionService.OnUserConnected(user, this);
            return true;
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            var user = await userManager.GetUserAsync((ClaimsPrincipal)Context.User);
            connectionService.OnUserDisconnected(user, this);
            await base.OnDisconnected(stopCalled);
        }
    }
}
