using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class DatasetCategoryElementEntityTypeConfiguration : IEntityTypeConfiguration<DatasetCategoryElement>
    {
        public void Configure(EntityTypeBuilder<DatasetCategoryElement> configuration)
        {
            configuration.ToTable("DatasetCategoryElement");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.DatasetCategoryId)
                .IsRequired()
                .HasColumnName("DatasetCategory_Id");

            configuration.Property(e => e.DatasetElementId)
                .IsRequired()
                .HasColumnName("DatasetElement_Id");

            configuration.Property(c => c.FriendlyName)
                .HasMaxLength(150);

            configuration.Property(c => c.Help)
                .HasMaxLength(350);

            configuration.Property(c => c.Uid)
                .HasMaxLength(10)
                .HasColumnName("UID");

            configuration.Property(c => c.FieldOrder)
                .IsRequired();

            configuration.Property(c => c.Acute)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(c => c.Chronic)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(c => c.System)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(c => c.Public)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasOne(d => d.DatasetCategory)
                .WithMany(p => p.DatasetCategoryElements)
                .HasForeignKey(d => d.DatasetCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.DatasetElement)
                .WithMany(p => p.DatasetCategoryElements)
                .HasForeignKey(d => d.DatasetElementId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => new { e.DatasetCategoryId, e.DatasetElementId }).IsUnique(true);
            configuration.HasIndex(e => e.DatasetCategoryId);
            configuration.HasIndex(e => e.DatasetElementId);
        }
    }
}
