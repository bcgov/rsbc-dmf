namespace pdipadapter.Data;

using pdipadapter.Data.ef;
using pdipadapter.Models;
using pdipadapter.Models.Lookups;
using Microsoft.EntityFrameworkCore;
using NodaTime;

public class JumDbContext : DbContext
{
    private readonly IClock clock;

    public JumDbContext(DbContextOptions<JumDbContext> options, IClock clock) : base(options) => this.clock = clock;

    public DbSet<Player> Players { get; set; } = default!;
    public DbSet<JustinUser> Users { get; set; } = default!; 

    //public DbSet<ParticipantModel> Participants { get; set; } = default!;
    public DbSet<JustinRole> Roles { get; set; } = default!;
    public DbSet<JustinPerson> People { get; set; } = default!;
    public DbSet<JustinIdentityProvider> IdentityProviders { get; set; } = default!;
    public DbSet<JustinAgency> Agencies { get; set; } = default!;
    public DbSet<JustinAgencyAssignment> AgencyAssignments { get; set; } = default!;
    public DbSet<JustinPartyType> PartyTypes { get; set; } = default!;
    //public DbSet<DigitalParticipantModel> DigitalParticipants { get; set; } = default!;
    public override int SaveChanges()
    {
        this.ApplyAudits();

        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.ApplyAudits();

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(JumDbContext).Assembly);
    }

    private void ApplyAudits()
    {
        this.ChangeTracker.DetectChanges();
        var updated = this.ChangeTracker.Entries()
            .Where(x => x.Entity is BaseAuditable
                && (x.State == EntityState.Added || x.State == EntityState.Modified));

        var currentInstant = this.clock.GetCurrentInstant();

        foreach (var entry in updated)
        {
            entry.CurrentValues[nameof(BaseAuditable.Modified)] = currentInstant;

            if (entry.State == EntityState.Added)
            {
                entry.CurrentValues[nameof(BaseAuditable.Created)] = currentInstant;
            }
            else
            {
                entry.Property(nameof(BaseAuditable.Created)).IsModified = false;
            }
        }
    }

}
