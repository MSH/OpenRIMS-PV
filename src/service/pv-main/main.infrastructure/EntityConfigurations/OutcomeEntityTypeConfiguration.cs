using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class OutcomeEntityTypeConfiguration : IEntityTypeConfiguration<Outcome>
    {
        public void Configure(EntityTypeBuilder<Outcome> configuration)
        {
            configuration.ToTable("Outcome");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
