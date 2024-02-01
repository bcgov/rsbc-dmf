using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Pssg.DocumentStorageAdapter.Helpers;
using Rsbc.Dmf.CaseManagement.Helpers;
using Rsbc.Dmf.DriverPortal.Api;
using Rsbc.Dmf.DriverPortal.Api.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    /// <summary>
    /// web application factory used for testing HttpClient
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly IConfiguration _configuration;

        public CustomWebApplicationFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddTransient<DocumentFactory>();

                // add policy but then fake success always, the RequireClaim is bypassed
                services.AddAuthorization(options =>
                {
                    // policy needed to bypass services with attribute [Authorize(Policy = Policy.Driver)]
                    options.AddPolicy(Policy.Driver, policy => policy.RequireClaim(UserClaimTypes.DriverId));
                });
                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

                services.AddControllersWithViews().AddJsonOptions(x =>
                {
                    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

                services.AddAutoMapperSingleton(LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole()));

                // setup http context with mocked user claims
                var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
                var context = new DefaultHttpContext();
                var user = new ClaimsPrincipal();
                var userId = _configuration["USER_SUBJECT"] ?? "SubjectId";
                var driverId = _configuration["DRIVER_WITH_USER"] ?? "DriverId";
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Sid, userId),
                    new Claim(UserClaimTypes.DriverId, driverId),
                    new Claim(ClaimTypes.Email, "Email"),
                    new Claim(ClaimTypes.Upn, $"ExternalSystemUserId"),
                    new Claim(ClaimTypes.GivenName, "FirstName"),
                    new Claim(ClaimTypes.Surname, "LastName")
                };
                user.AddIdentity(new ClaimsIdentity(claims));
                context.User = user;
                mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
                services.AddTransient(x => mockHttpContextAccessor.Object);

                services.AddTransient<IUserService, UserService>();
                
                // document storage client
                string documentStorageAdapterURI = _configuration["DOCUMENT_STORAGE_ADAPTER_URI"];
                if (string.IsNullOrEmpty(documentStorageAdapterURI))
                {
                    // add the mock
                    var documentStorageAdapterClient = DocumentStorageHelper.CreateMock(_configuration);
                    services.AddTransient(_ => documentStorageAdapterClient);
                }
                else
                {
                    services.AddDocumentStorageClient(_configuration);
                }

                // case management client
                string cmsAdapterURI = _configuration["CMS_ADAPTER_URI"];
                if (string.IsNullOrEmpty(cmsAdapterURI))
                {
                    // setup from Mock
                    var caseManagerClient = CmsHelper.CreateMock(_configuration);
                    services.AddTransient(_ => caseManagerClient);
                }
                else
                {
                    services.AddCaseManagementAdapterClient(_configuration);
                }
            });

            builder
                .UseSolutionRelativeContentRoot("")
                .UseEnvironment("Staging")
                .UseConfiguration(_configuration);
        }

        public class FakePolicyEvaluator : IPolicyEvaluator
        {
            public virtual async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
            {
                var principal = new ClaimsPrincipal();
                principal.AddIdentity(
                    new ClaimsIdentity(new[] {
                        new Claim(UserClaimTypes.DriverId, "DriverId")
                    }
                ));

                return await Task.FromResult(
                    AuthenticateResult.Success(new AuthenticationTicket(principal, new AuthenticationProperties(), null)));
            }

            public virtual async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy,
                AuthenticateResult authenticationResult, HttpContext context, object resource)
            {
                return await Task.FromResult(PolicyAuthorizationResult.Success());
            }
        }
    }
}
