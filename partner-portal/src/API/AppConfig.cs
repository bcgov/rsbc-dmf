using Microsoft.Extensions.Hosting;

public class AppConfig
{
    public bool IsProduction() => EnvironmentName == Environments.Production;
    public bool IsDevelopment() => EnvironmentName == Environments.Development;
    public string EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    public string CorsOrigins { get; set; }
    public CmsAdapterConfig CmsAdapter { get; set; }
    public SplunkConfig Splunk { get; set; }

    public class CmsAdapterConfig
    {
        public string ServerUrl { get; set; }
        public bool ValidateServerCertificate { get; set; }
    }

    public class SplunkConfig
    {
        public string Url { get; set; }
        public string Token { get; set; }
    }
}
