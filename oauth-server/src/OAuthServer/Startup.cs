using IdentityModel.Client;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OAuthServer
{
    public class Startup
    {
        private const string HealthCheckReadyTag = "ready";
        private readonly IWebHostEnvironment environment;
        private readonly IConfiguration configuration;

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            this.environment = environment;
            this.configuration = configuration;
        }

        private readonly string MyPolicy = "_myPolicy";

        public void ConfigureServices(IServiceCollection services)
        {
            var dpBuilder = services.AddDataProtection();
            var keyRingPath = configuration.GetValue<string>("KEY_RING_PATH");
            if (!string.IsNullOrWhiteSpace(keyRingPath))
            {
                //configure data protection folder for key sharing
                dpBuilder.PersistKeysToFileSystem(new DirectoryInfo(keyRingPath));
            }

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyPolicy,
                    builder =>
                    {
                        builder.WithOrigins("https://roadsafetybcportal-dev.apps.silver.devops.gov.bc.ca",
                                            "https://roadsafetybcportal-test.apps.silver.devops.gov.bc.ca",
                                            "https://roadsafetybcportal-train.apps.silver.devops.gov.bc.ca",
                                            "https://localhost:3020",
                                            "http://localhost:3020")
                               .WithMethods("PUT", "POST", "DELETE", "GET", "OPTIONS");
                    });
            });

            services.AddControllersWithViews();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var config = JsonSerializer.Deserialize<Config>(File.ReadAllText("./Data/config.json"), new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } });
            var builder = services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    options.UserInteraction.LoginUrl = "~/login";
                    options.UserInteraction.LogoutUrl = "~/logout";

                    if (!string.IsNullOrEmpty(configuration["ISSUER_URI"])) options.IssuerUri = configuration["ISSUER_URI"];
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

            //store the oidc key in the key ring persistent volume
            var keyPath = Path.Combine(new DirectoryInfo(keyRingPath ?? "./Data").FullName, "oidc_key.jwk");

            //add key as signing key
            builder.AddDeveloperSigningCredential(filename: keyPath);

            //add key as encryption key to oidc jwks endpoint that is used by BCSC to encrypt tokens
            var encryptionKey = Microsoft.IdentityModel.Tokens.JsonWebKey.Create(File.ReadAllText(keyPath));
            encryptionKey.Use = "enc";
            builder.AddValidationKey(new SecurityKeyInfo { Key = encryptionKey });

            services.AddOidcStateDataFormatterCache();
            services.AddDistributedMemoryCache();
            services.AddResponseCompression();
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddAuthentication()
           .AddOpenIdConnect("bcsc", options =>
           {
               // Note: Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectHandler  doesn't handle JWE correctly
               // See https://github.com/dotnet/aspnetcore/issues/4650 for more information
               // When BCSC user info payload is encrypted, we need to load the user info manually in OnTokenValidated event below
               // IdentityModel.Client also doesn't support JWT userinfo responses, so the following code takes care of this manually
               options.GetClaimsFromUserInfoEndpoint = false;

               configuration.GetSection("identityproviders:bcsc").Bind(options);

               options.ResponseType = OpenIdConnectResponseType.Code;
               options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
               options.SignOutScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

               //add required scopes
               options.Scope.Add("profile");
               options.Scope.Add("address");
               options.Scope.Add("email");

               //set the tokens decrypting key
               options.TokenValidationParameters.TokenDecryptionKey = encryptionKey;

               options.Events = new OpenIdConnectEvents
               {
                   OnTokenValidated = async ctx =>
                   {
                       var oidcConfig = await ctx.Options.ConfigurationManager.GetConfigurationAsync(CancellationToken.None);

                       //set token validation parameters
                       var validationParameters = ctx.Options.TokenValidationParameters.Clone();
                       validationParameters.IssuerSigningKeys = oidcConfig.JsonWebKeySet.GetSigningKeys();
                       validationParameters.ValidateLifetime = false;
                       validationParameters.ValidateIssuer = false;
                       var userInfoRequest = new UserInfoRequest
                       {
                           Address = oidcConfig.UserInfoEndpoint,
                           Token = ctx.TokenEndpointResponse.AccessToken
                       };
                       //set the userinfo response to be JWT
                       userInfoRequest.Headers.Accept.Clear();
                       userInfoRequest.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/jwt"));

                       //request userinfo claims through the backchannel
                       var response = await ctx.Options.Backchannel.GetUserInfoAsync(userInfoRequest, CancellationToken.None);
                       if (response.IsError && response.HttpStatusCode == HttpStatusCode.OK)
                       {
                           //handle encrypted userinfo response...
                           if (response.HttpResponse.Content?.Headers?.ContentType?.MediaType == "application/jwt")
                           {
                               var handler = new JwtSecurityTokenHandler();
                               if (handler.CanReadToken(response.Raw))
                               {
                                   handler.ValidateToken(response.Raw, validationParameters, out var token);
                                   var jwe = token as JwtSecurityToken;
                                   ctx.Principal.AddIdentity(new ClaimsIdentity(new[] { new Claim("userInfo", jwe.Payload.SerializeToJson()) }));
                               }
                           }
                           else
                           {
                               //...or fail
                               ctx.Fail(response.Error);
                           }
                       }
                       else if (response.IsError)
                       {
                           //handle for all other failures
                           ctx.Fail(response.Error);
                       }
                       else
                       {
                           //handle non encrypted userinfo response
                           ctx.Principal.AddIdentity(new ClaimsIdentity(new[] { new Claim("userInfo", response.Json.GetRawText()) }));
                       }
                   },
                   OnUserInformationReceived = async ctx =>
                   {
                       //handle userinfo claim mapping when options.GetClaimsFromUserInfoEndpoint = true
                       await Task.CompletedTask;
                       ctx.Principal.AddIdentity(new ClaimsIdentity(new[]
                       {
                              new Claim("userInfo", ctx.User.RootElement.GetRawText())
                       }));
                   }
               };
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
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
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

            app.UsePathBase(configuration["BASE_PATH"] ?? "");
            app.UseRouting();

            app.UseCors(MyPolicy);

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