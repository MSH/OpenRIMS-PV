using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class LabTestUnitEntityTypeConfiguration : IEntityTypeConfiguration<LabTestUnit>
    {
        public void Configure(EntityTypeBuilder<LabTestUnit> configuration)
        {
            configuration.ToTable("LabTestUnit");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
