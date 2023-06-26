using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class PatientStatusEntityTypeConfiguration : IEntityTypeConfiguration<PatientStatus>
    {
        public void Configure(EntityTypeBuilder<PatientStatus> configuration)
        {
            configuration.ToTable("PatientStatus");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.HasIndex("Description").IsUnique(false);
        }
    }
}
