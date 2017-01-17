using Heisenslaught.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heisenslaught.Hubs
{
    public class ProtectedServerEventHub : Hub
    {
        private ServerEventService eventService;

        public ProtectedServerEventHub(ServerEventService eventService)
        {
            this.eventService = eventService;
        }
            
        public void AddListeners()
        {

        }

        public void RemoveListeners()
        {

        }
    }
}
