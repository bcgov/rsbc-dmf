using pdipadapter.Infrastructure.HttpClients.Keycloak;
using pdipadapter.Models;
using pdipadapter.Models.Lookups;

namespace pdipadapter.Data.ef;

[Table(nameof(JustinIdentityProvider))]
public class JustinIdentityProvider : BaseAuditable
{
    [Key]
    public long IdentityProviderId { get; set; }
    /// <summary>
    /// The idps internal guid from keycloak
    /// </summary>
    /// <example>fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [Required]
    public Guid InternalId { get; set; }
    /// <summary>
    /// The alias of keycloak Idp
    /// </summary>
    /// <example>bcsc</example>
    [Required]
    [Column("Keycloak_idp_alias")]
    public string Alias { get; set; } = string.Empty;
    /// <summary>
    /// The alias of keycloak Idp
    /// </summary>
    /// <example>oidc,saml</example>
    [Required]
    public string ProviderId { get; set; } = string.Empty;
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    [Required]
    public bool IsActive { get; set; }
    public string TokenUrl { get; set; } = string.Empty;
    public string AuthUrl { get; set; } = string.Empty;

    public static implicit operator JustinIdentityProvider?(Guid? v)
    {
        throw new NotImplementedException();
    }
}

public class IdentityProviderDataGenerator : ILookupDataGenerator<JustinIdentityProvider>
{
    private readonly IKeycloakAdministrationClient _keycloak;
    public IdentityProviderDataGenerator(IKeycloakAdministrationClient keycloak)
    {
        _keycloak = keycloak;
    }

    public IEnumerable<JustinIdentityProvider> Generate()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<JustinIdentityProvider>> GenerateAsync()
    {
        var idps = await _keycloak.IdentityProviders();
        return idps?.Select(t => new JustinIdentityProvider
        {
            //IdentityProviderId = t.ProviderId
            Alias = t.Alias,
            ProviderId = t.ProviderId,
            InternalId = t.InternalId,
            IsActive = t.Enabled,
            Name = t.DisplayName,


        }); ;
        //return default;
    }
}
