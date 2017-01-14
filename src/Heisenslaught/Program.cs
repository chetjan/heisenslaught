using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Heisenslaught
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            var builder = new WebHostBuilder()
                .UseKestrel(options =>
                {
                    // This section may only be needed in devmode, it should most likely be handled by ngix when deployed
                    options.NoDelay = true;
                   // options.UseHttps("testCert.pfx", "testPassword");
                    options.UseConnectionLogging();
                    
                });
            
            // needed?
            builder.UseUrls("http://localhost:64808", "https://localhost:44301");
            
                
            var host = builder.UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
