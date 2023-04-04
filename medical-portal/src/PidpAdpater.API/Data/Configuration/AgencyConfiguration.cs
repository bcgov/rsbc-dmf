using pdipadapter.Data.ef;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace pdipadapter.Data.Configuration;

public class AgencyConfiguration : IEntityTypeConfiguration<JustinAgency>
{
    public void Configure(EntityTypeBuilder<JustinAgency> builder)
    {
        builder.HasMany(u => u.Users)
            .WithOne(a => a.Agency)
            .HasForeignKey(a => a.AgencyId);
    }
}
public class AgencyLoadConfiguration : LookupTableConfiguration<JustinAgency, AgencyDataGenerator>
{
}
