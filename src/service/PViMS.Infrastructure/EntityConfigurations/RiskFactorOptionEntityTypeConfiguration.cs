using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class RiskFactorOptionEntityTypeConfiguration : IEntityTypeConfiguration<RiskFactorOption>
    {
        public void Configure(EntityTypeBuilder<RiskFactorOption> configuration)
        {
            configuration.ToTable("RiskFactorOption");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Criteria)
                .IsRequired()
                .HasMaxLength(250);

            configuration.Property(e => e.Display)
                .HasMaxLength(30);

            configuration.Property(e => e.OptionName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.RiskFactorId)
                .IsRequired()
                .HasColumnName("RiskFactor_Id");

            configuration.HasOne(d => d.RiskFactor)
                .WithMany(p => p.Options)
                .HasForeignKey(d => d.RiskFactorId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex("Display").IsUnique(true);
            configuration.HasIndex(e => e.RiskFactorId);
        }
    }
}
