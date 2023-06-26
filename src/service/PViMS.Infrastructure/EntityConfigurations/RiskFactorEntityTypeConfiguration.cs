using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class RiskFactorEntityTypeConfiguration : IEntityTypeConfiguration<RiskFactor>
    {
        public void Configure(EntityTypeBuilder<RiskFactor> configuration)
        {
            configuration.ToTable("RiskFactor");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Criteria)
                .IsRequired();

            configuration.Property(e => e.Display)
                .HasMaxLength(20);

            configuration.Property(e => e.FactorName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.IsSystem)
                .IsRequired();

            configuration.Property(e => e.Active)
                .IsRequired();

            configuration.HasIndex("FactorName").IsUnique(true);
        }
    }
}
