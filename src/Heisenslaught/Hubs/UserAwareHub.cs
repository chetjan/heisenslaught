using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Server;
using Heisenslaught.Services;
using Microsoft.AspNetCore.Identity;
using Heisenslaught.Models.Users;
using System.Security.Claims;

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

           OnUserConnected();
           return base.OnConnected();
        }

        protected async Task OnUserConnected()
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
