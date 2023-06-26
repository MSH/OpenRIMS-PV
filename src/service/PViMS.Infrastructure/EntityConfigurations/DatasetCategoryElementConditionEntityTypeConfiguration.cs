using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetCategoryElementConditionEntityTypeConfiguration : IEntityTypeConfiguration<DatasetCategoryElementCondition>
    {
        public void Configure(EntityTypeBuilder<DatasetCategoryElementCondition> configuration)
        {
            configuration.ToTable("DatasetCategoryElementCondition");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ConditionId)
                .IsRequired()
                .HasColumnName("Condition_Id");

            configuration.Property(e => e.DatasetCategoryElementId)
                .IsRequired()
                .HasColumnName("DatasetCategoryElement_Id");

            configuration.HasOne(d => d.Condition)
                .WithMany(p => p.DatasetCategoryElementConditions)
                .HasForeignKey(d => d.ConditionId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasOne(d => d.DatasetCategoryElement)
                .WithMany(p => p.DatasetCategoryElementConditions)
                .HasForeignKey(d => d.DatasetCategoryElementId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => new { e.ConditionId, e.DatasetCategoryElementId }).IsUnique(true);
            configuration.HasIndex(e => e.ConditionId);
            configuration.HasIndex(e => e.DatasetCategoryElementId);
        }
    }
}
