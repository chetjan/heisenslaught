using Heisenslaught.Hubs;
using Heisenslaught.Models.Users;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heisenslaught.Services
{
    public class ServerEvent{
        public string context;
        public string type;
        public object data;
    }

    public class ServerEventService
    {
        private readonly Dictionary<string, Func<Hub, bool>> eventContexts = new Dictionary<string, Func<Hub, bool>>();
        private readonly IServiceProvider serviceProvider;
        private IHubConnectionContext<ServerEventHub> hubContext;
       
        public ServerEventService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            registerEventContext("system.broadcast.all");
            registerEventContext("system.broadcast.users");
        }

        private IHubConnectionContext<ServerEventHub> Hub
        {
            get {
                if(hubContext == null)
                {
                    hubContext = (IHubConnectionContext<ServerEventHub>)serviceProvider.GetService(typeof(IHubConnectionContext<ServerEventHub>));
                }
                return hubContext;
            }
        }


        public void registerEventContext(string name, Func<Hub,bool> permissionCheck = null)
        {
            lock (eventContexts)
            {
                if (!eventContexts.ContainsKey(name))
                {
                    eventContexts.Add(name, permissionCheck);
                }
                else
                {
                    // throw error;
                }
            }
        }

        public void AddEventContextListeners(Hub hub, string[] contexts)
        {
            lock (eventContexts)
            {
                for (var i = 0; i < contexts.Length; i++)
                {
                    var name = contexts[i];
                    if (eventContexts.ContainsKey(name))
                    {
                        var check = eventContexts[name];
                        if(check == null || check(hub))
                        {
                            hub.Groups.Add(hub.Context.ConnectionId, name);
                        }
                    }
                }
            }
        }
   
        public void RemoveEventContextListeners(Hub hub, string[] contexts)
        {
            lock (eventContexts)
            {
                for (var i = 0; i < contexts.Length; i++)
                {
                    var name = contexts[i];
                    if (eventContexts.ContainsKey(name))
                    {
                        hub.Groups.Remove(hub.Context.ConnectionId, name);
                    }
                }
            }
        }

        public void Emit(string context, object data)
        {
            Hub.Group(context).Clients.All.emit(new ServerEvent
            {
                context = context,
                type = data.GetType().Name,
                data = data
            });
        }
    }
}
