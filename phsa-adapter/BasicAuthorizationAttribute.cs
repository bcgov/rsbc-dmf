using Microsoft.AspNetCore.Authorization;

namespace Rsbc.Dmf.PhsaAdapter
{
    public class BasicAuthorizationAttribute : AuthorizeAttribute
    {
        public BasicAuthorizationAttribute()
        {
            Policy = "BasicAuthentication";
        }
    }
}