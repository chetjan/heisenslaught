using Heisenslaught.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heisenslaught.Hubs
{
    public class ServerEventHub : Hub
    {

        private ServerEventService eventService;

        public ServerEventHub(ServerEventService eventService)
        {
            this.eventService = eventService;     
        }

        public void AddListeners(string[] contexts)
        {
            eventService.AddEventContextListeners(this, contexts);
        }

        public void RemoveListeners(string[] contexts)
        {
            eventService.RemoveEventContextListeners(this, contexts);
        }
    }
}
