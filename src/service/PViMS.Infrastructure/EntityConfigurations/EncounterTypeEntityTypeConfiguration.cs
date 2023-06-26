using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class EncounterTypeEntityTypeConfiguration : IEntityTypeConfiguration<EncounterType>
    {
        public void Configure(EntityTypeBuilder<EncounterType> configuration)
        {
            configuration.ToTable("EncounterType");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(c => c.Help)
                .HasMaxLength(250);

            configuration.Property(c => c.Chronic)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
