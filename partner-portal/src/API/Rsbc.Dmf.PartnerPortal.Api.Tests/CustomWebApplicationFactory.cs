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
//using Rsbc.Dmf.CaseManagement.Helpers;
//using RSBC.DMF.MedicalPortal.API;
//using RSBC.DMF.MedicalPortal.API.Services;
//using Moq;
//using Pssg.DocumentStorageAdapter.Helpers;
//using System.Collections.Generic;
using System.Security.Claims;
//using System.Threading.Tasks;
//using static RSBC.DMF.MedicalPortal.API.Auth.AuthConstant;
//using RSBC.DMF.MedicalPortal.API.Model;

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

    // NOTE configuration needed for within HttpClient services
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // add policy but then fake success always, the RequireClaim is bypassed
            services.AddAuthorization(options =>
            {
                // policy needed to bypass services with attribute [Authorize(Policy = Policies.MedicalPractitioner)]
                options.AddPolicy(Policies.Oidc, policy => policy.RequireAssertion(assert => true));
                //options.AddPolicy(Policies.MedicalPractitioner, policy => policy.RequireClaim(ClaimTypes.Role, Roles.Practitoner));
            });
            services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

            services.AddControllers(options =>
            {
                //options.Filters.Add(new HttpResponseExceptionFilter());
            });

            //services.AddTransient<IUserService, UserService>();
            //services.AddTransient<DocumentFactory>();

            //services.AddAutoMapperSingleton();

            // setup http context with mocked user claims
            //if (_configuration["TEST_PIDP_USER_ID"] != null)
            //{
            //    var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            //    var context = new DefaultHttpContext();
            //    var user = new ClaimsPrincipal();
            //    var claims = new List<Claim>
            //    {
            //        new Claim(Claims.PreferredUsername, _configuration["TEST_PIDP_USER_ID"]),
            //        new Claim(Claims.LoginIds, $"[\"{_configuration["TEST_LOGIN_IDS"]}\"]"),
            //        new Claim(ClaimTypes.GivenName, "John"),
            //        new Claim(ClaimTypes.Surname, "Smith"),
            //        new Claim(Claims.Email, "john.smith@mailinator.com"),
            //        // not sure if this is the correct role, should be whatever Api uses for ClaimsIdentity.RoleClaimType
            //        new Claim(Claims.Roles, Policies.MedicalPractitioner),
            //        //new Claim(Claims.Endorsements, null)
            //    };
            //    user.AddIdentity(new ClaimsIdentity(claims));
            //    context.User = user;
            //    mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(context);
            //    services.AddTransient(x => mockHttpContextAccessor.Object);
            //    services.AddHttpContextAccessor();
            //}

            // document storage client
            //string documentStorageAdapterURI = _configuration["DOCUMENT_STORAGE_ADAPTER_URI"];
            //if (string.IsNullOrEmpty(documentStorageAdapterURI))
            //{
            //    // add the mock
            //    var documentStorageAdapterClient = DocumentStorageHelper.CreateMock(_configuration);
            //    services.AddTransient(_ => documentStorageAdapterClient);
            //}
            //else
            //{
            //    services.AddDocumentStorageClient(_configuration);
            //}

            //// case management client
            //string cmsAdapterURI = _configuration["CMS_ADAPTER_URI"];
            //if (string.IsNullOrEmpty(cmsAdapterURI))
            //{
            //    // setup from Mock
            //    var caseManagerClient = CmsHelper.CreateMock(_configuration);
            //    services.AddTransient(_ => caseManagerClient);
            //}
            //else
            //{
            //    services.AddCaseManagementAdapterClient(_configuration);
            //}

            //// pidp adapter client
            //string pidpAdapterURI = _configuration["PIDP_ADAPTER_URI"];
            //if (string.IsNullOrEmpty(pidpAdapterURI))
            //{
            //    // setup from Mock
            //    //var pidpAdapterClient = PidpHelper.CreateMock(_configuration);
            //    //services.AddTransient(_ => pidpAdapterClient);
            //}
            //else
            //{
            //    services.AddPidpAdapterClient(_configuration);
            //}
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
