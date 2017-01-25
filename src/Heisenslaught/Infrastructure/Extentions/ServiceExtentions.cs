using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace Heisenslaught.Infrastructure.Extentions
{
    public static class ServiceExtentions
    {
        private static readonly HashSet<Type> services =  new HashSet<Type>();


        public static IServiceCollection AddService(this IServiceCollection serviceCollection, Type serviceType, Type implimentationType)
        {
            services.Add(serviceType);
            return serviceCollection.AddSingleton(serviceType, implimentationType);
        }

        public static IServiceCollection AddService<TType, TImpl>(this IServiceCollection serviceCollection)
        {
            return AddService(serviceCollection, typeof(TType), typeof(TImpl));
        }

        public static IServiceCollection AddService<TImpl>(this IServiceCollection serviceCollection)
        {
            return AddService(serviceCollection, typeof(TImpl), typeof(TImpl));
        }


        public static IApplicationBuilder UseServices(this IApplicationBuilder app)
        {
            foreach(var t in services)
            {
                app.ApplicationServices.GetRequiredService(t);
            }
            return app;
        }


    }
}
