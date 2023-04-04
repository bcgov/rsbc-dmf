using pdipadapter.Data.ef;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace pdipadapter.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<JustinUser>
{
    public virtual void Configure(EntityTypeBuilder<JustinUser> builder)
    {
    
        builder.HasKey(e => e.UserId);

        builder.HasIndex(u => u.UserName).IsUnique();

        builder.HasIndex(u => u.ParticipantId).IsUnique();

        //builder.HasMany(r => r.UserRoles)
        //    .WithOne(u => u.User)
        //    .HasForeignKey(f => f.UserId)
        //    .IsRequired();

        builder.HasOne(d => d.Person)
            .WithOne(p => p.User)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .IsRequired();

        builder.HasOne(a => a.Agency)
            .WithMany(u => u.Users)
            .HasForeignKey(a => a.AgencyId);



    }
}
