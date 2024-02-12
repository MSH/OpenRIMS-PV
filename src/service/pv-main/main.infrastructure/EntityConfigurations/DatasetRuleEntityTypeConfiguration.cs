using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class DatasetRuleEntityTypeConfiguration : IEntityTypeConfiguration<DatasetRule>
    {
        public void Configure(EntityTypeBuilder<DatasetRule> configuration)
        {
            configuration.ToTable("DatasetRule");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.DatasetElementId)
                .HasColumnName("DatasetElement_Id");

            configuration.Property(e => e.DatasetId)
                .HasColumnName("Dataset_Id");

            configuration.Property(c => c.RuleType)
                .HasConversion(x => (int)x, x => (DatasetRuleType)x);

            configuration.Property(c => c.RuleActive)
                .IsRequired();

            configuration.HasOne(d => d.DatasetElement)
                .WithMany(p => p.DatasetRules)
                .HasForeignKey(d => d.DatasetElementId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.Dataset)
                .WithMany(p => p.DatasetRules)
                .HasForeignKey(d => d.DatasetId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => e.DatasetElementId);
            configuration.HasIndex(e => e.DatasetId);
        }
    }
}
