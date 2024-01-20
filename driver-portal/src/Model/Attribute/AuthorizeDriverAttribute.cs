using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Rsbc.Dmf.DriverPortal.Api.Services;

namespace Rsbc.Dmf.DriverPortal.Api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeDriverAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService<AuthorizeDriver>();
        }

        public class AuthorizeDriver : ActionFilterAttribute, IAsyncAuthorizationFilter
        {
            private readonly IUserService _userService;

            public AuthorizeDriver(IUserService userService)
            {
                _userService = userService;
            }

            public async Task OnAuthorizationAsync(AuthorizationFilterContext filterContext)
            {
                if (!filterContext.RouteData.Values.ContainsKey("driverId"))
                {
                    filterContext.Result = new BadRequestObjectResult("parameter driverId missing.");
                    return;
                }

                var driverId = filterContext.RouteData.Values["driverId"] as string;

                var isAuthorizedResponse = await _userService.IsDriverAuthorized(driverId);
                if (!isAuthorizedResponse)
                    filterContext.Result = new UnauthorizedResult();
            }
        }
    }
}
