using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class TreatmentOutcomeEntityTypeConfiguration : IEntityTypeConfiguration<TreatmentOutcome>
    {
        public void Configure(EntityTypeBuilder<TreatmentOutcome> configuration)
        {
            configuration.ToTable("TreatmentOutcome");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
