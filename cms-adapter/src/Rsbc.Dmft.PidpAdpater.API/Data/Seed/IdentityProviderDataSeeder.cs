using pdipadapter.Data.ef;
using pdipadapter.Infrastructure.HttpClients.Keycloak;
using Microsoft.EntityFrameworkCore;

namespace pdipadapter.Data.Seed;

public class IdentityProviderDataSeeder
{
    private readonly IKeycloakAdministrationClient _keycloakClient;
    private readonly ILogger<IdentityProviderDataSeeder> _logger;
    private readonly JumDbContext _context;

    public IdentityProviderDataSeeder(IKeycloakAdministrationClient keycloakClient
                                        , ILogger<IdentityProviderDataSeeder> logger
                                        , JumDbContext context)
    {
        _keycloakClient = keycloakClient;
        _logger = logger;
        _context = context;
    }
    public async Task Seed()
    {
            _context.Database.EnsureCreated();
            await _context.Database.MigrateAsync();

        if (!await _context.IdentityProviders.AnyAsync())
        {
            _logger.LogWarning("Adding IDPs from Keycloak");
            var idps = await _keycloakClient.IdentityProviders();
            var c = idps.Select(t => new JustinIdentityProvider
            {
                Alias = t.Alias,
                ProviderId = t.ProviderId,
                InternalId = t.InternalId,
                IsActive = t.Enabled,
                Name = t.DisplayName,
                TokenUrl = t.Config.TokenUrl,
                AuthUrl = t.Config.AuthorizationUrl
            });
            await _context.IdentityProviders.AddRangeAsync(c);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Keycloaks IDPs added to DataSource.");
        }
    }
        
      
    
}
