namespace pdipadapter.Infrastructure.HttpClients.Keycloak;
using System.Text.Json;
public class Client
{
    /// <summary>
    /// ID referenced in URIs and tokens
    /// </summary>
    public string? ClientId { get; set; }

    /// <summary>
    /// Guid-like unique identifier
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Display name
    /// </summary>
    public string? Name { get; set; }
}

public class Role
{
    public bool? ClientRole { get; set; }
    public bool? Composite { get; set; }
    public string? ContainerId { get; set; }
    public string? Description { get; set; }
    public string? Id { get; set; }
    public string? Name { get; set; }
}

/// <summary>
/// This is not the entire Keycloak User Representation! See https://www.keycloak.org/docs-api/5.0/rest-api/index.html#_userrepresentation.
/// This is a sub-set of the properties so we don't accidentally overwrite anything when doing the PUT.
/// </summary>
public class UserRepresentation
{
    public string? Email { get; set; }
    public Dictionary<string, string[]> Attributes { get; set; } = new();

    //internal void SetLdapOrgDetails(LdapLoginResponse.OrgDetails orgDetails) => this.SetAttribute("org_details", JsonSerializer.Serialize(orgDetails, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));

    public void SetPhone(string phone) => SetAttribute("phone", phone);

    public void SetPhoneNumber(string phoneNumber) => SetAttribute("phoneNumber", phoneNumber);

    public void SetPhoneExtension(string phoneExtension) => SetAttribute("phoneExtension", phoneExtension);

    private void SetAttribute(string key, string value) => Attributes[key] = new string[] { value };
}
public class IdentityProvider
{
    public string Alias { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public Guid InternalId { get; set; }
    public bool Enabled { get; set; }
    public string ProviderId { get; set; } = string.Empty;
    public Config Config { get; set; } = new Config();

}
public class Config
{
    public string TokenUrl { get; set; } = string.Empty;
    public string AuthorizationUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
}