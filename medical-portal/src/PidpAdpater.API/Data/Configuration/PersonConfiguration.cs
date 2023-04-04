using pdipadapter.Data.ef;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace pdipadapter.Data.Configuration;

public class PersonConfiguration : IEntityTypeConfiguration<JustinPerson>
{
    public virtual void Configure(EntityTypeBuilder<JustinPerson> builder)
    {
        //builder.HasOne(u => u.User)
        //    .WithOne(x => x.Person)
        //    .OnDelete(DeleteBehavior.Cascade);
        builder.HasKey(e => e.PersonId);
    }
}
