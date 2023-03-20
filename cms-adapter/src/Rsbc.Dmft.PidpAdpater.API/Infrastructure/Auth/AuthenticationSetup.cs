using Confluent.Kafka;
using pdipadapter.Extensions;
using pdipadapter.Kafka.Producer;
using pdipadapter.Kafka.Producer.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace pdipadapter.Infrastructure.Auth
{
    public static class AuthenticationSetup
    {
        //public IConfiguration Configuration { get; }
        public static IServiceCollection AddKeycloakAuth(this IServiceCollection services, pdipadapterConfiguration config)
        {
            //Configuration = configuration;
            services.ThrowIfNull(nameof(services));
            config.ThrowIfNull(nameof(config));

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = config.KafkaCluster.BoostrapServers,
                Acks = Acks.All,               
                SaslMechanism = SaslMechanism.Plain,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslUsername = config.KafkaCluster.ClientId,
                SaslPassword = config.KafkaCluster.ClientSecret,
                EnableIdempotence = true
            };

            services.AddSingleton(producerConfig);
            services.AddSingleton(typeof(IKafkaProducer<,>), typeof(KafkaProducer<,>));

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = config.Keycloak.RealmUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = Clients.PidpApi;
                options.MetadataAddress = config.Keycloak.WellKnownConfig;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Tu2iOytd-ptnPEMBw-k15sKfycQZnY_Iuq2O96oj1Pw")),
                    ValidAlgorithms = new List<string>() { "RS256" },
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context => {
                        if (context.Principal?.Identity is ClaimsIdentity identity
                            && identity.IsAuthenticated)
                        {
                            // Flatten the Resource Access claim
                            identity.AddClaims(identity.GetResourceAccessRoles(Clients.PidpExternalApi)
                                .Select(role => new Claim(ClaimTypes.Role, role)));
                            identity.AddClaims(identity.GetResourceAccessRoles(Clients.LicenceStatus)
                                .Select(role => new Claim(ClaimTypes.Role, role)));
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.OnStarting(async () =>
                        {
                            context.NoResult();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            string response =
                            JsonConvert.SerializeObject("The access token provided is not valid.");
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                                response =
                                    JsonConvert.SerializeObject("The access token provided has expired.");
                            }
                            await context.Response.WriteAsync(response);
                        });
                   
                        //context.HandleResponse();
                        //context.Response.WriteAsync(response).Wait();
                        return Task.CompletedTask;

                    },
                    OnForbidden = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        if (string.IsNullOrEmpty(context.Error))
                            context.Error = "invalid_token";
                        if (string.IsNullOrEmpty(context.ErrorDescription))
                            context.ErrorDescription = "This request requires a valid JWT access token to be provided";

                        return context.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            error = context.Error,
                            error_description = context.ErrorDescription
                        }));
                    }
                };
            });
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.AddPolicy("Administrator", policy => policy.Requirements.Add(new RealmAccessRoleRequirement("administrator")));
                options.AddPolicy(Policies.MedicalPractitioner, policy => policy
                    .RequireAuthenticatedUser()
                    .RequireRole(Claims.IdentityProvider, Roles.Practitoner, Roles.Moa));
            });
            return services;
            
        }
    }

}
