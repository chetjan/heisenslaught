using Heisenslaught.Models.Users;
using Heisenslaught.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Heisenslaught.Hubs
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
           OnUserConnectedAsync().Wait();
           return base.OnConnected();
        }

        protected async Task OnUserConnectedAsync()
        {
            var user = await userManager.GetUserAsync((ClaimsPrincipal)Context.User);
            connectionService.OnUserConnected(user, this);
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            var user = await userManager.GetUserAsync((ClaimsPrincipal)Context.User);
            connectionService.OnUserDisconnected(user, this);
            await base.OnDisconnected(stopCalled);
        }
    }
}
