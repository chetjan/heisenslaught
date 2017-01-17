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
    
        private readonly HashSet<string> publicContexts = new HashSet<string>();
        private readonly Dictionary<string, Func<Hub, bool>> protectedContexts = new Dictionary<string, Func<Hub, bool>>();

        private readonly IHubConnectionContext<PublicServerEventHub> publicHub;
        private readonly IHubConnectionContext<ProtectedServerEventHub> protectedHub;


        public ServerEventService(IHubConnectionContext<PublicServerEventHub> publicHub, IHubConnectionContext<ProtectedServerEventHub> protectedHub)
        {
            this.protectedHub = protectedHub;
            this.publicHub = publicHub;
        }

        public void registerPublicEventContext(string name)
        {
            lock (publicContexts)
            {
                if (!publicContexts.Add(name))
                {
                    // throw error;
                }
            }
        }

        public void registerProtectedEventContext(string name, Func<Hub,bool> permissionCheck = null)
        {
            lock (protectedContexts)
            {
                if (protectedContexts.ContainsKey(name))
                {
                    protectedContexts.Add(name, permissionCheck);
                }
                else
                {
                    // throw error;
                }
            }
        }


        public void ConnectToPublicContexts(Hub hub, string[] contexts)
        {
            lock (publicContexts)
            {
                for(var i = 0; i < contexts.Length; i++)
                {
                    var name = contexts[i];
                    if (publicContexts.Contains(name))
                    {
                        hub.Groups.Add(hub.Context.ConnectionId, name);
                    }
                }
            }
        }
        public void ConnectToProtectedContexts(Hub hub, string[] contexts)
        {
            lock (protectedContexts)
            {
                for (var i = 0; i < contexts.Length; i++)
                {
                    var name = contexts[i];
                    if (protectedContexts.ContainsKey(name))
                    {
                        var check = protectedContexts[name];
                        if(check == null || check(hub))
                        {
                            hub.Groups.Add(hub.Context.ConnectionId, name);
                        }
                    }
                }
            }
        }

        public void DisconnectFromPublicContexts(Hub hub, string[] contexts)
        {
            lock (publicContexts)
            {
                for (var i = 0; i < contexts.Length; i++)
                {
                    var name = contexts[i];
                    if (publicContexts.Contains(name))
                    {
                        hub.Groups.Remove(hub.Context.ConnectionId, name);
                    }
                }
            }
        }

        public void DisconnectFromProtectedContexts(Hub hub, string[] contexts)
        {
            lock (protectedContexts)
            {
                for (var i = 0; i < contexts.Length; i++)
                {
                    var name = contexts[i];
                    if (protectedContexts.ContainsKey(name))
                    {
                        hub.Groups.Remove(hub.Context.ConnectionId, name);
                    }
                }
            }
        }

        public void EmitPublic(string context, object data)
        {
            publicHub.Group(context).Clients.All.emit(new ServerEvent {
                context = context,
                type = data.GetType().Name,
                data = data
            });
        }

        public void EmitProtected(string context, object data)
        {
            protectedHub.Group(context).Clients.All.emit(new ServerEvent
            {
                context = context,
                type = data.GetType().Name,
                data = data
            });
        }
    }
}
