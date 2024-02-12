using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Aggregates.MetaFormAggregate;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class MetaFormCategoryEntityTypeConfiguration : IEntityTypeConfiguration<MetaFormCategory>
    {
        public void Configure(EntityTypeBuilder<MetaFormCategory> configuration)
        {
            configuration.ToTable("MetaFormCategory");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.MetaFormId)
                .IsRequired()
                .HasColumnName("MetaForm_Id");

            configuration.Property(e => e.MetaTableId)
                .IsRequired()
                .HasColumnName("MetaTable_Id");

            configuration.Property(e => e.MetaFormCategoryGuid)
                .IsRequired()
                .HasColumnName("metaformcategory_guid");

            configuration.Property(e => e.CategoryName)
                .IsRequired()
                .HasMaxLength(150);

            configuration.Property(e => e.Help)
                .HasMaxLength(500);

            configuration.Property(e => e.Icon)
                .HasMaxLength(50);

            configuration.HasOne(d => d.MetaForm)
                .WithMany(p => p.Categories)
                .HasForeignKey(d => d.MetaFormId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => e.MetaFormId);
        }
    }
}