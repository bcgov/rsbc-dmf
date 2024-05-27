using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RSBC.DMF.MedicalPortal.API.Services;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Reflection;
using Grpc.Net.Client;
using Pssg.DocumentStorageAdapter;
using System.Data;
using static RSBC.DMF.MedicalPortal.API.Auth.AuthConstant;
using System.IdentityModel.Tokens.Jwt;
using RSBC.DMF.MedicalPortal.API.Auth.Extension;
using System.Text.Json;
using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Common;


namespace RSBC.DMF.MedicalPortal.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            this.configuration = configuration;
            this.environment = environment;
        }
        private IConfiguration configuration { get; }
        private const string HealthCheckReadyTag = "ready";
        private readonly IHostEnvironment environment;

        public void ConfigureServices(IServiceCollection services)
        {
            // TODO change this later, this is not standard configuration, used driver-portal as a reference
            var config = this.InitializeConfiguration(services);

            services.AddKeycloakWebApiAuthentication(
                keycloakOptions => 
                {
                    keycloakOptions.Realm = config.Keycloak.Config.Realm;
                    keycloakOptions.Audience = config.Keycloak.Config.Audience;
                    keycloakOptions.AuthServerUrl = config.Keycloak.Config.Url;
                    keycloakOptions.Credentials = new KeycloakClientInstallationCredentials
                    {                  
                        Secret = configuration.GetValue<string>("Keycloak:Secret")
                    };
                },
                jwtBearerOptions => 
                { 
                    jwtBearerOptions.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context => await OnTokenValidatedAsync(context),
                        OnAuthenticationFailed = context =>
                        {
                            Log.Error(context.Exception, "Error validating bearer token");
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                // TODO confirm by using invalid clientsecret, might need to add policy "Bearer" to force authentication check above
                // oauth authentication
                //options.AddPolicy("OAuth", policy =>
                //{
                //    policy.RequireAuthenticatedUser().AddAuthenticationSchemes("introspection");
                //    //policy.RequireClaim("scope", "doctors-portal-api");
                //});

                // TODO uncomment this and add it to all policies, also update the user secrets scope
                // check if we need scope medical-portal-ui, if not, rename to medical-portal
                //policy.RequireClaim("scope", "medical-portal-api");

                options.AddPolicy(
                    Policies.MedicalPractitioner,
                    policy => policy
                        .RequireAuthenticatedUser()
                        .RequireRole(Claims.IdentityProvider, Roles.Practitoner, Roles.Moa)
                );

                options.AddPolicy(Policies.Enrolled, policy => policy
                    .RequireAuthenticatedUser()
                    // TODO uncomment to validate the user is enrolled, we may need to make sure that the claim name has not changed since POC
                    //.RequireRole(Claims.IdentityProvider, Roles.Dmft)
                    );
            });

            services.AddControllers(options =>
            {
                options.Filters.Add(new HttpResponseExceptionFilter());
            });                

            services.AddSwaggerGen(c =>
            {
                // add Xml comments to the swagger docs
                //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlPath));
                //c.SchemaFilter<EnumTypesSchemaFilter>(xmlPath);
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RSBC.DMF.MedicalPortal.API", Version = "v1" });
            });            

            var dpBuilder = services.AddDataProtection();
            var keyRingPath = configuration.GetValue("DATAPROTECTION__PATH", string.Empty);
            if (!string.IsNullOrWhiteSpace(keyRingPath))
            {
                //configure data protection folder for key sharing
                dpBuilder.PersistKeysToFileSystem(new DirectoryInfo(keyRingPath));
            }
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                //set known network of forward headers
                options.ForwardLimit = 2;
                var configvalue = configuration.GetValue("app:knownNetwork", string.Empty)?.Split('/');
                if (configvalue.Length == 2)
                {
                    var knownNetwork = new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Parse(configvalue[0]), int.Parse(configvalue[1]));
                    options.KnownNetworks.Add(knownNetwork);
                }
            });
            services.AddCors();/* opts => opts.AddDefaultPolicy(policy =>
            {
                // try to get array of origins from section array
                var corsOrigins = configuration.GetSection("app:cors:origins").GetChildren().Select(c => c.Value).ToArray();
                // try to get array of origins from value
                if (!corsOrigins.Any()) corsOrigins = configuration.GetValue("app:cors:origins", string.Empty).Split(',');
                corsOrigins = corsOrigins.Where(o => !string.IsNullOrWhiteSpace(o)).ToArray();
                if (corsOrigins.Any())
                {
                    policy.SetIsOriginAllowedToAllowWildcardSubdomains().WithOrigins(corsOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                }
            }));*/
            services.AddDistributedMemoryCache();
            services.AddResponseCompression();
            services.AddHealthChecks().AddCheck("Medical Portal API", () => HealthCheckResult.Healthy("OK"), new[] { HealthCheckReadyTag });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.AddHttpContextAccessor();

            // Add Case Management Service

            // Add Case Management System (CMS) Adapter 

            services.AddCaseManagementAdapterClient(configuration);

            // Add Document Storage Adapter

            string documentStorageAdapterURI = configuration["DOCUMENT_STORAGE_ADAPTER_URI"];

            if (!string.IsNullOrEmpty(documentStorageAdapterURI))
            {
                var httpClientHandler = new HttpClientHandler();
                if (!environment.IsProduction()) // Ignore certificate errors in non-production modes.  
                                                 // This allows you to use OpenShift self-signed certificates for testing.
                {
                    // Return `true` to allow certificates that are untrusted/invalid                    
                    httpClientHandler.ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                }

                var httpClient = new HttpClient(httpClientHandler);
                // set default request version to HTTP 2.  Note that Dotnet Core does not currently respect this setting for all requests.
                httpClient.DefaultRequestVersion = HttpVersion.Version20;

                var initialChannel = GrpcChannel.ForAddress(documentStorageAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });

                var initialClient = new DocumentStorageAdapter.DocumentStorageAdapterClient(initialChannel);
                // call the token service to get a token.
                var tokenRequest = new Pssg.DocumentStorageAdapter.TokenRequest
                {
                    Secret = configuration["DOCUMENT_STORAGE_ADAPTER_JWT_SECRET"]
                };

                var tokenReply = initialClient.GetToken(tokenRequest);

                if (tokenReply != null && tokenReply.ResultStatus == Pssg.DocumentStorageAdapter.ResultStatus.Success)
                {
                    // Add the bearer token to the client.
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenReply.Token}");

                    var channel = GrpcChannel.ForAddress(documentStorageAdapterURI, new GrpcChannelOptions { HttpClient = httpClient, MaxReceiveMessageSize = null, MaxSendMessageSize = null });

                    services.AddTransient(_ => new DocumentStorageAdapter.DocumentStorageAdapterClient(channel));
                }
            }

            services.AddTransient<ICaseQueryService, CaseService>();
            services.AddTransient<IUserService, UserService>();
            services.AddAutoMapperSingleton();
        }

        private async Task OnTokenValidatedAsync(TokenValidatedContext context)
        {
            if (context.Principal?.Identity is ClaimsIdentity identity
            && identity.IsAuthenticated)
            {
                // Flatten the Resource Access claim
                identity.AddClaims(identity.GetResourceAccessRoles(Clients.License)
                    .Select(role => new Claim(ClaimTypes.Role, role)));

                var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                context.Principal = await userService.Login(context.Principal);
            }
        }
        private MedicalPortalConfiguration InitializeConfiguration(IServiceCollection services)
        {
            var config = new MedicalPortalConfiguration();
            this.configuration.Bind(config);

            services.AddSingleton(config);

            Log.Logger.Information("### App Version:{0} ###", Assembly.GetExecutingAssembly().GetName().Version);
            Log.Logger.Information("### PIdP Configuration:{0} ###", JsonSerializer.Serialize(config));

            return config;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging(opts =>
            {
                opts.GetLevel = ExcludeHealthChecks;
                opts.EnrichDiagnosticContext = (diagCtx, httpCtx) =>
                {
                    diagCtx.Set("User", httpCtx.User.FindFirstValue(ClaimTypes.Upn));
                    diagCtx.Set("Host", httpCtx.Request.Host);
                    diagCtx.Set("UserAgent", httpCtx.Request.Headers["User-Agent"].ToString());
                    diagCtx.Set("RemoteIP", httpCtx.Connection.RemoteIpAddress.ToString());
                    diagCtx.Set("ConnectionId", httpCtx.Connection.Id);
                    diagCtx.Set("Forwarded", httpCtx.Request.Headers["Forwarded"].ToString());
                    diagCtx.Set("ContentLength", httpCtx.Response.ContentLength);
                };
            });

            app.UseHealthChecks("/hc/ready", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecks("/hc/live", new HealthCheckOptions
            {
                // Exclude all checks and return a 200-Ok.
                Predicate = _ => false
            });

            app.UseAuthentication();

            app.UseCors();

            app.UseRouting();
            app.UseResponseCompression();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers()
                    .RequireAuthorization(/*Policies.MedicalPractitioner/*, Policies.Enrolled/*, "OAuth"*/);
            });
        }

        private static LogEventLevel ExcludeHealthChecks(HttpContext ctx, double _, Exception ex) =>
            ex != null
                ? LogEventLevel.Error
                : ctx.Response.StatusCode >= (int)HttpStatusCode.InternalServerError
                    ? LogEventLevel.Error
                    : ctx.Request.Path.StartsWithSegments("/hc", StringComparison.InvariantCultureIgnoreCase)
                        ? LogEventLevel.Verbose
                        : LogEventLevel.Information;
    }

}