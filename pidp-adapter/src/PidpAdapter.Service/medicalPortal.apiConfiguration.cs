namespace PidpAdapter.Infrastructure.Auth;

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
}
