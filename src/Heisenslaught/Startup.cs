using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.HttpOverrides;
using Heisenslaught.Models.Users;
using Heisenslaught.Persistence.User;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using AspNet.Security.OAuth.BattleNet;
using Heisenslaught.Config;
using Microsoft.Extensions.Options;

namespace Heisenslaught
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder =  new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("/opt/Heisenslaught/appsettings.json", optional: true)
                .AddJsonFile($"/opt/Heisenslaught/appsettings.{env.EnvironmentName}.json", optional: true);

           
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }



            // This method gets called by the runtime. Use this method to add services to the container.
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            // mongo strores
            services.Configure<MongoSettings>(Configuration.GetSection("MongoDb"));

            services.AddSingleton<IUserStore<HSUser>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoSettings>>();
                var client = new MongoClient(options.Value.ConnectionString);
                var db = client.GetDatabase(options.Value.Database);
                var logger = provider.GetService<ILoggerFactory>();
                return new HSUserStore(db, logger, "users");
            });

            services.AddSingleton<IRoleStore<HSRole>>(provider =>
            {
                var options = provider.GetService<IOptions<MongoSettings>>();
                var client = new MongoClient(options.Value.ConnectionString);
                var db = client.GetDatabase(options.Value.Database);
                var logger = provider.GetService<ILoggerFactory>();
                return new HSRoleStore(db, logger, "roles");
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Identity managers/validators
            services.AddSingleton<IUserValidator<HSUser>, HSUserValidator>();
            services.AddSingleton<RoleManager<HSRole>, RoleManager<HSRole>>();
            services.AddSingleton<UserManager<HSUser>, UserManager<HSUser>>();
            services.AddScoped<SignInManager<HSUser>, SignInManager<HSUser>>();

            // initialize Identity
            services.AddIdentity<HSUser, HSRole>()
                .AddDefaultTokenProviders();
            services.AddOptions();
    
            services.AddMvc();
            services.AddSignalR();
            services.AddRouting(options => {
                options.LowercaseUrls = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();

            // set up login providers
            app.UseIdentity();
          
            app.UseBattleNetAuthentication(options =>
            {
                options.Region = BattleNetAuthenticationRegion.America;
                options.DisplayName = "BattleNet";
                options.ClientId = Configuration["Authentication:BattleNet:ClientID"];
                options.ClientSecret = Configuration["Authentication:BattleNet:ClientSecret"];
            });

            app.UseWebSockets();
            app.UseSignalR();

            app.UseMvc(routes =>
            {
                
                routes.MapRoute(
                    name: "auth",
                    template: "auth/{action}/",
                    defaults: new { controller = "Auth", action = "Index" }
                );

                // If not other route matches serve angular. NOTE: This needs to always be the last route
                routes.MapRoute(
                    name: "angular",
                    template: "{*any}",
                    defaults: new { controller = "Angular", action = "Index" }
                );
            });
        }
    }
}
