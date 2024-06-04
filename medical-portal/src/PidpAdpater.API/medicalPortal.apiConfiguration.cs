namespace OneHealthAdapter.Infrastructure.Auth;

public class Configuration
{
    public static bool IsProduction() => EnvironmentName == Environments.Production;
    public static bool IsDevelopment() => EnvironmentName == Environments.Development;
    private static readonly string? EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    public SettingsConfiguration Settings { get; set; } = new();

    public class SettingsConfiguration
    {
        public CorsConfiguration Cors { get; set; } = new();
    }

    public class CorsConfiguration
    {
        public string AllowedOrigins { get; set; } = string.Empty;
    }

    public KeycloakConfiguration Keycloak { get; set; } = new();

    public PidpEndorsementAPIConfiguration PidpEndorsementAPI { get; set; } = new();

    // ------- Configuration Objects -------

    public class PidpEndorsementAPIConfiguration
    {
        public string Url { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string TokenUrl { get; set; } = string.Empty;
    }

    public class KeycloakConfiguration
    {
        public string RealmUrl { get; set; } = string.Empty;
        public string WellKnownConfig => KeycloakUrls.WellKnownConfig(this.RealmUrl);
        public string TokenUrl => KeycloakUrls.Token(this.RealmUrl);
        public string AdministrationUrl { get; set; } = string.Empty;
        public string AdministrationClientId { get; set; } = string.Empty;
        public string AdministrationClientSecret { get; set; } = string.Empty;
        public string HcimClientId { get; set; } = string.Empty;
    }
}
