using Microsoft.AspNetCore.SignalR;

namespace Heisenslaught.Infrastructure.ServerEvents
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
