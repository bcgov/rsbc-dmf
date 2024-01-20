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
using static Rsbc.Dmf.DriverPortal.Api.AuthorizeDriverAttribute;

namespace Rsbc.Dmf.DriverPortal.Tests
{
    /// <summary>
    /// web application factory used for testing HttpClient
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly IConfiguration _configuration;
        private readonly bool _isAuthorizationEnabled;

        public CustomWebApplicationFactory(IConfiguration configuration, bool isAuthorizationEnabled)
        {
            _configuration = configuration;
            _isAuthorizationEnabled = isAuthorizationEnabled;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
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
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Sid, "QPSETRIJEOMSARHBRNRPBVAIFL65V3YI"),
                    new Claim(ClaimTypes.Email, "Email"),
                    new Claim(ClaimTypes.Upn, $"ExternalSystemUserId"),
                    new Claim(ClaimTypes.GivenName, "FirstName"),
                    new Claim(ClaimTypes.Surname, "LastName")
                };
                user.AddIdentity(new ClaimsIdentity(claims));
                context.User = user;
                mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
                services.AddTransient(x => mockHttpContextAccessor.Object);

                if (!_isAuthorizationEnabled)
                {
                    var userService = new Mock<IUserService>();
                    userService.Setup(x => x.IsDriverAuthorized(It.IsAny<string>())).ReturnsAsync(true);
                    services.AddTransient(x => userService.Object);
                }
                else
                {
                    services.AddTransient<IUserService, UserService>();
                }
                
                services.AddTransient<AuthorizeDriver>();

                // document storage client
                string documentStorageAdapterURI = _configuration["DOCUMENT_STORAGE_ADAPTER_URI"];
                if (true || string.IsNullOrEmpty(documentStorageAdapterURI))
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
    }
}