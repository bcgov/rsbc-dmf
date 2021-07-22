using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Text.Json;

namespace OAuthServer
{
    public class Startup
    {
        private const string HealthCheckReadyTag = "ready";
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var config = JsonSerializer.Deserialize<Config>(File.ReadAllText("./Data/config.json"));

            var builder = services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    options.UserInteraction.LoginUrl = "~/login";
                })

                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlite(connectionString);
                    options.EnableTokenCleanup = true;
                })
                .AddInMemoryApiScopes(config.ApiScopes)
                .AddInMemoryClients(config.Clients)
                .AddInMemoryIdentityResources(config.IdentityResources)
                .AddInMemoryApiResources(config.ApiResources)
                .AddInMemoryCaching()
                ;

            builder.AddDeveloperSigningCredential(filename: "./Data/tempkey.jwk");
            services.AddOidcStateDataFormatterCache();
            services.AddDistributedMemoryCache();
            services.AddResponseCompression();
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddAuthentication()
                .AddOpenIdConnect("bcsc", options =>
                {
                    options.ClientId = "ca.bc.gov.pssg.dmfw.dev";
                    options.ClientSecret = "geuRHzWFbMfsO8jo0gLW";
                    options.MetadataAddress = "https://idtest.gov.bc.ca/login/.well-known/openid-configuration";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.UseTokenLifetime = true;
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };

                    //add required scopes
                    options.Scope.Add("address");
                    options.Scope.Add("email");
                });

            services.AddHealthChecks().AddCheck("OAuth Server ", () => HealthCheckResult.Healthy("OK"), new[] { HealthCheckReadyTag });
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            app.UseXXssProtection(opts => opts.EnabledWithBlockMode());

            app.UseXfo(xfo => xfo.SameOrigin());
            app.UseXRobotsTag(options => options.NoIndex().NoFollow());

            app.UseCsp(opts => opts.BlockAllMixedContent());

            app.UseResponseCompression();
            app.UseCookiePolicy();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHealthChecks("/hc/ready", new HealthCheckOptions()
                {
                    Predicate = (check) => check.Tags.Contains(HealthCheckReadyTag)
                });

                endpoints.MapHealthChecks("/hc/live", new HealthCheckOptions()
                {
                    Predicate = (_) => false
                });
            });
        }
    }
}