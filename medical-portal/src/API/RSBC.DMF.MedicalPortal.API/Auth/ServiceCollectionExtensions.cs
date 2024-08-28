using Keycloak.AuthServices.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using RSBC.DMF.MedicalPortal.API.Services;
using System.Security.Claims;
using static RSBC.DMF.MedicalPortal.API.Auth.AuthConstant;
using RSBC.DMF.MedicalPortal.API.Auth.Extension;
using System.Net;
using Serilog;
using Microsoft.AspNetCore.Authorization;

namespace RSBC.DMF.MedicalPortal.API.Auth
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuth(this IServiceCollection services, MedicalPortalConfiguration config)
        {
            services.AddKeycloakWebApiAuthentication(
                keycloakOptions =>
                {
                    keycloakOptions.Realm = config.Keycloak.Config.Realm;
                    keycloakOptions.Audience = config.Keycloak.Config.Audience;
                    keycloakOptions.AuthServerUrl = config.Keycloak.Config.Url;
                    keycloakOptions.VerifyTokenAudience = false;
                },
                jwtBearerOptions =>
                {
                    jwtBearerOptions.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context => await OnTokenValidatedAsync(context, config),
                        OnAuthenticationFailed = context =>
                        {
                            // TODO use logger factory instead of Serilog
                            Log.Error(context.Exception, "Error validating bearer token");
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    Policies.Oidc,
                    policy => policy
                        // confirm this is working by using a bad secret, currently the secret is not being validated
                        .RequireAuthenticatedUser().AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    // TODO verify if we need to add scope medical-portal-ui and medical-portal-api or if just medical-portal will do, in other projects there are api and ui
                    // the below does not work, since the scope claim looks something like "email profile openid". This problem has already been solved, research the proper way to handle scope
                    // need to add the scope to keycloak admin UI before we can add the scope to FE, which would pass the scope claim to the BE
                    //.RequireClaim(Claims.Scope, "medical-portal")
                );

                // attribute policies
                options.AddPolicy(
                    Policies.MedicalPractitioner,
                    policy => policy
                        .RequireAuthenticatedUser()
                        .RequireRole(Claims.IdentityProvider, Roles.Practitoner, Roles.Moa));

                options.AddPolicy(Policies.Enrolled, policy => policy
                    .RequireAuthenticatedUser()
                    .RequireRole(Claims.IdentityProvider, Roles.Dmft));

                // run-time policies 
                options.AddPolicy(
                    Policies.NetworkPractitioner,
                    policy => policy
                        .RequireAuthenticatedUser()
                        .Requirements.Add(new NetworkPractitionerRequirement()));

                options.AddPolicy(
                    Policies.MedicalPractitioner,
                    policy => policy
                        .RequireAuthenticatedUser()
                        .Requirements.Add(new PractitionerRequirement()));
            });

            services.AddSingleton<IAuthorizationHandler, NetworkPractitionerAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, PractitionerAuthorizationHandler>();

            return services;
        }

        private static async Task OnTokenValidatedAsync(TokenValidatedContext context, MedicalPortalConfiguration configuration)
        {
            if (context.Principal?.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
            {
                // Flatten the Resource Access claim

                var licenseRoles = identity
                    .GetResourceAccessRoles(Clients.License)
                    .Select(role => new Claim(identity.RoleClaimType, role));
                if (licenseRoles.Any())
                {
                    identity.AddClaims(licenseRoles);
                }
                else
                {
                    var moaRoleClaim = new Claim(identity.RoleClaimType, Roles.Moa);
                    identity.AddClaim(moaRoleClaim);
                }

                // TODO check for DMFT enrolled, currently allowing non-enrolled users access
                var enrolledRoles = identity
                    .GetResourceAccessRoles(Clients.DmftStatus)
                    .Select(role => new Claim(identity.RoleClaimType, role));
                identity.AddClaims(enrolledRoles);

                if (configuration.FeatureSimpleAuth)
                {
                    identity.AddClaim(new Claim(identity.RoleClaimType, Roles.Practitoner));
                    identity.AddClaim(new Claim(identity.RoleClaimType, Roles.Dmft));
                }

                // TODO I think this is wrong, we should only need to call this once but this is validating on every request
                var claims = identity.Claims;
                var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                context.Principal = await userService.Login(context.Principal);
            }
        }
    }
}
