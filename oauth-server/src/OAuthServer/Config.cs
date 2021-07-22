using IdentityServer4.Models;
using System.Collections.Generic;

namespace OAuthServer
{
    public class Config
    {
        public IEnumerable<IdentityResource> IdentityResources { get; set; }

        public IEnumerable<ApiScope> ApiScopes { get; set; }

        public IEnumerable<ApiResource> ApiResources { get; set; }

        public IEnumerable<Client> Clients { get; set; }
    }
}