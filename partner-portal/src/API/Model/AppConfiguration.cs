using Microsoft.Extensions.Configuration;

public class AppConfiguration
{
    public KeycloakConfiguration Keycloak { get; set; }

    public AppConfiguration(IConfiguration configuration)
    {
        Keycloak = new KeycloakConfiguration
        {
            RealmUrl = configuration["KEYCLOAK_REALM_URL"],
            Config = new KeycloakConfiguration.KeycloakConfig
            {
                Url = configuration["KEYCLOAK_AUTH_URL"],
                Audience = configuration["KEYCLOAK_AUDIENCE"],
                Realm = configuration["KEYCLOAK_REALM"],
                ClientId = configuration["KEYCLOAK_CLIENT_ID"]
            },
            InitOptions = new KeycloakConfiguration.KeycloakInitOptions
            {
                OnLoad = "check-sso",
                Flow = "standard",
                ResponseMode = "fragment",
                PkceMethod = "S256",
                CheckLoginIframe = false
            }
        };
    }

    public class KeycloakConfiguration
    {
        public string RealmUrl { get; set; }
        public KeycloakConfig Config { get; set; }
        public KeycloakInitOptions InitOptions { get; set; }

        public class KeycloakConfig
        {
            public string Url { get; set; }
            public string Audience { get; set; }
            public string Realm { get; set; }
            public string ClientId { get; set; }
        }

        public class KeycloakInitOptions
        {
            public string OnLoad { get; set; }
            public string Flow { get; set; }
            public string ResponseMode { get; set; }
            public string PkceMethod { get; set; }
            public bool CheckLoginIframe { get; set; }
        }
    }
}
