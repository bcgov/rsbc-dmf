using Flurl;

namespace RSBC.DMF.MedicalPortal.API;

public class MedicalPortalConfiguration
{
    public SettingsConfiguration Settings { get; set; } = new();
    public KeycloakConfiguration Keycloak { get; set; } = new();

    public class SettingsConfiguration
    {
        public CorsConfiguration Cors { get; set; } = new();
    }

    public class CorsConfiguration
    {
        public string AllowedOrigins { get; set; }
    }

    public class KeycloakConfiguration
    {
        public string RealmUrl { get; set; }
        public string WellKnownConfig => KeycloakUrls.WellKnownConfig(this.RealmUrl);
        public string TokenUrl => KeycloakUrls.Token(this.RealmUrl);
        
        public KeycloakConfig Config { get; set; }
        public KeycloakInitOptions InitOptions { get; set; }

        public class KeycloakConfig {
            public string Url { get; set; }
            public string Realm { get; set; }
            public string ClientId { get; set; }
            public string Audience { get; set; }
        }

        public class KeycloakInitOptions
        {
            public string OnLoad { get; set; }
            public string Flow { get; set; }
            public string ResponseMode { get; set; }
            public string PkceMethod { get; set; }
        }
    }
}

public static class KeycloakUrls
{
    /// <summary>
    /// Returns the URL for OAuth token issuance.
    /// </summary>
    /// <param name="realmUrl">URL of the keycloak instance up to the realm name; I.e. "[base url]/auth/realms/[realm name]"</param>
    public static string Token(string realmUrl) => Url.Combine(realmUrl, "protocol/openid-connect/token");

    /// <summary>
    /// Returns the URL for the OAuth well-known config.
    /// </summary>
    /// <param name="realmUrl">URL of the keycloak instance up to the realm name; I.e. "[base url]/auth/realms/[realm name]"</param>
    public static string WellKnownConfig(string realmUrl) => Url.Combine(realmUrl, ".well-known/openid-configuration");
}
