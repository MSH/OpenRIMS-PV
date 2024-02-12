using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class DatasetCategoryConditionEntityTypeConfiguration : IEntityTypeConfiguration<DatasetCategoryCondition>
    {
        public void Configure(EntityTypeBuilder<DatasetCategoryCondition> configuration)
        {
            configuration.ToTable("DatasetCategoryCondition");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ConditionId)
                .IsRequired()
                .HasColumnName("Condition_Id");

            configuration.Property(e => e.DatasetCategoryId)
                .IsRequired()
                .HasColumnName("DatasetCategory_Id");

            configuration.HasOne(d => d.Condition)
                .WithMany(p => p.DatasetCategoryConditions)
                .HasForeignKey(d => d.ConditionId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.DatasetCategory)
                .WithMany(p => p.DatasetCategoryConditions)
                .HasForeignKey(d => d.DatasetCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => new { e.ConditionId, e.DatasetCategoryId }).IsUnique(true);
            configuration.HasIndex(e => e.ConditionId);
            configuration.HasIndex(e => e.DatasetCategoryId);
        }
    }
}
