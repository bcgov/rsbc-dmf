using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
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
                    options.UserInteraction.LogoutUrl = "~/logout";

                    if (!string.IsNullOrEmpty(Configuration["ISSUER_URI"])) options.IssuerUri = Configuration["ISSUER_URI"];
                })

                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlite(connectionString, sql => sql.MigrationsAssembly(typeof(Startup).Assembly.FullName));
                    options.EnableTokenCleanup = true;
                })
                .AddInMemoryApiScopes(config.ApiScopes)
                .AddInMemoryClients(config.Clients)
                .AddInMemoryIdentityResources(config.IdentityResources)
                .AddInMemoryApiResources(config.ApiResources)
                .AddInMemoryCaching()
                ;

            builder.AddDeveloperSigningCredential(filename: "./Data/tempkey.jwk");
            var encryptionKey = Microsoft.IdentityModel.Tokens.JsonWebKey.Create(File.ReadAllText("./Data/tempkey.jwk"));
            encryptionKey.Use = "enc";
            builder.AddValidationKey(new SecurityKeyInfo { Key = encryptionKey });

            services.AddOidcStateDataFormatterCache();
            services.AddDistributedMemoryCache();
            services.AddResponseCompression();
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddAuthentication()
                .AddOpenIdConnect("bcsc", options =>
                {
                    Configuration.GetSection("identityproviders:bcsc").Bind(options);
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.UseTokenLifetime = true;
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.SignOutScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
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
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseForwardedHeaders();
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            app.UseXXssProtection(opts => opts.EnabledWithBlockMode());

            //app.UseXfo(xfo => xfo.SameOrigin());
            app.UseXRobotsTag(options => options.NoIndex().NoFollow());

            app.UseCsp(opts => opts
                .DefaultSources(opts => opts.Self().CustomSources("https://*.gov.bc.ca", "https://*.bcgov"))
                .BlockAllMixedContent());

            app.UseResponseCompression();
            app.UseCookiePolicy();

            app.UsePathBase(Configuration["BASE_PATH"] ?? "");
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