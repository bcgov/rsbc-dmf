using pdipadapter.Data.ef;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace pdipadapter.Data.Configuration;

public class UserRoleConfiguration : IEntityTypeConfiguration<JustinUserRole>
{
    public virtual void Configure(EntityTypeBuilder<JustinUserRole> builder)
    {
        //builder.HasKey(e => e.UserRoleId);

        //builder.HasKey(x => new { x.UserId, x.RoleId });

        builder.HasOne(u => u.User)
            .WithMany(ur => ur.UserRoles)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne(r => r.Role)
            .WithMany(ur => ur.UserRoles)
            .HasForeignKey(f => f.RoleId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        
    }
}
