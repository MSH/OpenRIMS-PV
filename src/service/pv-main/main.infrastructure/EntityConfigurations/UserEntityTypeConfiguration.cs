using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> configuration)
        {
            configuration.ToTable("User");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.FirstName)
                .IsRequired();

            configuration.Property(e => e.LastName)
                .IsRequired();

            configuration.Property(e => e.UserName)
                .IsRequired();

            configuration.Property(e => e.AllowDatasetDownload)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(e => e.IdentityId)
                .IsRequired();

            configuration.Property(c => c.UserType)
                .HasConversion(x => (int)x, x => (UserType)x);

            configuration.HasIndex("UserName").IsUnique(true);
        }
    }
}
