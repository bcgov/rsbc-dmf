// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Models;
using System.Collections.Generic;

namespace OAuthServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("doctors-portal-api"),
                new ApiScope("phsa-adapter"),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                     new ApiResource("doctors-portal-api")
                     {
                         ApiSecrets = { new Secret("xB7grPZkf9AL6F3".Sha256()) },
                         Scopes = { "doctors-portal-api" } ,
                         UserClaims = new IdentityResources.Profile().UserClaims
                     },
                     new ApiResource("phsa-adapter")
                     {
                         ApiSecrets = { new Secret("tULXt8dAPPDKuG7".Sha256()) },
                         Scopes = { "phsa-adapter" },
                         UserClaims = new IdentityResources.Profile().UserClaims
                     },
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "doctors-portal-ui",
                    RequireClientSecret = false,
                    ClientName = "doctors-portal-ui",
                    AccessTokenType = AccessTokenType.Reference,
                    AllowedGrantTypes = GrantTypes.Code,
                    //AllowAccessTokensViaBrowser = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    //AlwaysSendClientClaims = true,
                    //AlwaysIncludeUserClaimsInIdToken = true,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RedirectUris = { "http://localhost:3200/" },

                    // AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "email", "doctors-portal-api", "phsa-adapter" },

                    AllowedCorsOrigins = { "http://localhost:3200" }
                },
            };
    }
}