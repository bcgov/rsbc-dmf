namespace Rsbc.Dmf.CaseManagement.Dynamics
{
    public class DynamicsOptions
    {
        public string DynamicsApiEndpoint { get; set; }
        public string DynamicsApiBaseUri { get; set; }
        public DynamicsAuthenticationType AuthenticationType { get; set; } = DynamicsAuthenticationType.OnPremise;
        public AdfsOptions Adfs { get; set; } = new AdfsOptions();
        public EntraIdOptions EntraId { get; set; } = new EntraIdOptions();
    }

    public enum DynamicsAuthenticationType
    {
        OnPremise,
        Cloud,
    }

    public class AdfsOptions
    {
        public string OAuth2TokenEndpoint { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ServiceAccountDomain { get; set; }
        public string ServiceAccountName { get; set; }
        public string ServiceAccountPassword { get; set; }
        public string ResourceName { get; set; }
    }

    public class EntraIdOptions
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ResourceName { get; set; }
    }
}
