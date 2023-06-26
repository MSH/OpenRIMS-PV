using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class MedicationFormEntityTypeConfiguration : IEntityTypeConfiguration<MedicationForm>
    {
        public void Configure(EntityTypeBuilder<MedicationForm> configuration)
        {
            configuration.ToTable("MedicationForm");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
