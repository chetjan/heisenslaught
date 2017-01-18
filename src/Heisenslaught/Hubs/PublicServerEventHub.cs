using Heisenslaught.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heisenslaught.Hubs
{
    public class PublicServerEventHub : Hub
    {

        private ServerEventService eventService;

        public PublicServerEventHub(
            //ServerEventService eventService
            )
        {
            ///this.eventService = eventService;
            var a = 123;
        }

        public void AddListeners()
        {

        }

        public void RemoveListeners()
        {

        }
    }
}
