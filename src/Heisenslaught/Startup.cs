using AspNet.Security.OAuth.BattleNet;
using Heisenslaught.Config;
using Heisenslaught.Models.Users;
using Heisenslaught.Persistence.Draft;
using Heisenslaught.Persistence.User;
using Heisenslaught.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;

using Heisenslaught.Extentions;
using Heisenslaught.Persistence.service;

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
                .AddJsonFile("/opt/heisenslaught/appsettings.json", optional: true)
                .AddJsonFile($"/opt/heisenslaught/appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

       

            // This method gets called by the runtime. Use this method to add services to the container.
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // configs
            services.Configure<MongoSettings>(Configuration.GetSection("MongoDb"));
            services.Configure<UserCreationSettings>(Configuration.GetSection("UserCreation"));

           
            // mongo strores
            services.AddSingleton<IRoleStore<HSRole>>(provider =>
            {
                var logger = provider.GetService<ILoggerFactory>();

                var options = provider.GetService<IOptions<MongoSettings>>();
                var client = new MongoClient(options.Value.ConnectionString);
                var db = client.GetDatabase(options.Value.Database);
                return new HSRoleStore(db, logger);
            });

            services.AddSingleton<IUserStore<HSUser>>(provider =>
            {
                var logger = provider.GetService<ILoggerFactory>();
                var roleManager = provider.GetService<RoleManager<HSRole>>();

                var options = provider.GetService<IOptions<MongoSettings>>();
                var client = new MongoClient(options.Value.ConnectionString);
                var db = client.GetDatabase(options.Value.Database);
                var store =  new HSUserStore(db, logger, roleManager);
               // services.AddSingleton()
                return store;
            });

            services.AddSingleton<IDraftStore>(provider => {
                var options = provider.GetService<IOptions<MongoSettings>>();
                var client = new MongoClient(options.Value.ConnectionString);
                var db = client.GetDatabase(options.Value.Database);
                return new DraftStore(db);
            });

            services.AddSingleton<DraftListViewStore>(provider =>
            {
                var options = provider.GetService<IOptions<MongoSettings>>();
                var client = new MongoClient(options.Value.ConnectionString);
                var db = client.GetDatabase(options.Value.Database);
                return new DraftListViewStore(db);
            });

            // services
            services.AddSingleton<IHubConnectionsService, HubConnectionsService>();
            services.AddSingleton<IDraftService, DraftService>();
            services.AddSingleton<IHeroDataService, HeroDataService>();
            services.AddSingleton<ServerEventService, ServerEventService>();
            


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Identity managers/validators
            services.AddSingleton<IUserValidator<HSUser>, HSUserValidator>();
            services.AddSingleton<RoleManager<HSRole>, RoleManager<HSRole>>();
            services.AddSingleton<UserManager<HSUser>, UserManager<HSUser>>();
            services.AddScoped<SignInManager<HSUser>, SignInManager<HSUser>>();

            services.AddService<DraftListViewGenerator>();

            // initialize Identity
            services.AddIdentity<HSUser, HSRole>(options=> {
                options.Cookies.ExternalCookie.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.Cookies.ApplicationCookie.ExpireTimeSpan = TimeSpan.FromHours(1);
            })
                .AddDefaultTokenProviders();
            services.AddOptions();


            services.AddMvc(options => {
                options.SslPort = 44301;
            });
            services.AddSignalR(options=> {
                options.Hubs.EnableDetailedErrors = true;
                
            });
           
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

            app.UseServices();

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
