using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.HttpOverrides;
using Heisenslaught.Models.Users;
using Heisenslaught.Persistence.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MongoDB.Driver;
using AspNetCore.Identity.MongoDB;
using AspNet.Security.OAuth.BattleNet;

namespace Heisenslaught
{
    public class Startup
    {


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            // mongo strores - not yet used
            services.AddSingleton<IUserStore<HSUser>>(provider =>
            {
                var client = new MongoClient("mongodb://localhost:27017");
                var db = client.GetDatabase("Heisenslaught");
                var logger = provider.GetService<ILoggerFactory>();
                return new HSUserStore(db, logger, "users");
            });
            
            services.AddSingleton<IRoleStore<HSRole>>(provider =>
            {
                var client = new MongoClient("mongodb://localhost:27017");
                var db = client.GetDatabase("Heisenslaught");
                var logger = provider.GetService<ILoggerFactory>();
                return new HSRoleStore(db, logger, "roles");
            });

            // not sure if this is needed - same results either way
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Identity managers - signin manager doesn't seem to be able to capture the info when you authenticate - see AuthController.callback
            
            services.AddSingleton<IUserValidator<HSUser>, HSUserValidator>();
            services.AddSingleton<RoleManager<HSRole>, RoleManager<HSRole>>();
            services.AddSingleton<UserManager<HSUser>, UserManager<HSUser>>();
            services.AddScoped<SignInManager<HSUser>, SignInManager<HSUser>>();

            // add identity stuff
            services.AddIdentity<HSUser, HSRole>()
              //  .AddUserValidator<HSUserValidator>()
                .AddDefaultTokenProviders();
            
            // not sure what this does
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
            // this passes the code parmater check but login info is null
            app.UseOAuthAuthentication(new OAuthOptions
               {
                    
                   AuthenticationScheme = "BattleNet22", // esentially the id
                   DisplayName = "Battle.net",
                   ClientId = "426fcv27yht4tu9a4a4qn45su9r35ynj",
                   ClientSecret = "b9C3AkTupPDW6BnRhw6SdJgJRQhtxHtA",
                   ClaimsIssuer = "BattleNet",
                   AuthorizationEndpoint = "https://us.battle.net/oauth/authorize",
                   TokenEndpoint = "https://us.battle.net/oauth/token",
                   UserInformationEndpoint = "https://us.api.battle.net/account/user",
                   CallbackPath = "/auth/battlenet/", // this is taken over by the oauth code, its not an endpoint in the controller
                   AutomaticAuthenticate = true,
                   SaveTokens = true
            });
            // works
            app.UseBattleNetAuthentication(options =>
            {
                options.Region = BattleNetAuthenticationRegion.Europe;
                options.DisplayName = "BattleNet";
                options.ClientId = "426fcv27yht4tu9a4a4qn45su9r35ynj";
                options.ClientSecret = "b9C3AkTupPDW6BnRhw6SdJgJRQhtxHtA";
            });

            // works
            app.UseGoogleAuthentication(new GoogleOptions {
                AuthenticationScheme = "google",
                ClientId = "761544041286-i9fb5mbn6i9bt7fct5n6k5jsesbrqfag.apps.googleusercontent.com",
                ClientSecret = "QY_41N3WjgMqvzpPN85TfVMW"
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
