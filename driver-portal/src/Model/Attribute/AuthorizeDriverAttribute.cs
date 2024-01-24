using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Rsbc.Dmf.DriverPortal.Api.Services;

namespace Rsbc.Dmf.DriverPortal.Api
{
    // TODO delete this file without crashing github build
    // it builds locally and dockerfile, weird bug, clean didn't fix
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeDriverAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;
        public string ParameterName = "driverId";

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var authorizeDriverActionFilter = serviceProvider.GetService<AuthorizeDriver>();
            authorizeDriverActionFilter.ParameterName = ParameterName;
            return authorizeDriverActionFilter;
        }

        public class AuthorizeDriver : ActionFilterAttribute, IAsyncAuthorizationFilter
        {
            public string ParameterName = "driverId";

            private readonly IUserService _userService;

            public AuthorizeDriver(IUserService userService)
            {
                _userService = userService;
            }

            public async Task OnAuthorizationAsync(AuthorizationFilterContext filterContext)
            {
                //if (!filterContext.RouteData.Values.ContainsKey(ParameterName))
                //{
                //    filterContext.Result = new BadRequestObjectResult($"parameter {ParameterName} missing.");
                //    return;
                //}

                //var driverId = filterContext.RouteData.Values[ParameterName] as string;
                //var isAuthorizedResponse = await _userService.IsDriverAuthorized(driverId);
                //if (!isAuthorizedResponse)
                //    filterContext.Result = new UnauthorizedResult();
            }
        }
    }
}
