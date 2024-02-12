using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Aggregates.MetaFormAggregate;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class MetaFormCategoryAttributeEntityTypeConfiguration : IEntityTypeConfiguration<MetaFormCategoryAttribute>
    {
        public void Configure(EntityTypeBuilder<MetaFormCategoryAttribute> configuration)
        {
            configuration.ToTable("MetaFormCategoryAttribute");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.MetaFormCategoryId)
                .IsRequired()
                .HasColumnName("MetaFormCategory_Id");

            configuration.Property(e => e.MetaFormCategoryAttributeGuid)
                .IsRequired()
                .HasColumnName("metaformcategory_guid");

            configuration.Property(e => e.CustomAttributeConfigurationId)
                .IsRequired()
                .HasColumnName("Configuration_Id");

            configuration.Property(e => e.Label)
                .IsRequired()
                .HasMaxLength(150);

            configuration.Property(e => e.Help)
                .HasMaxLength(500);

            configuration.HasOne(d => d.MetaFormCategory)
                .WithMany(p => p.Attributes)
                .HasForeignKey(d => d.MetaFormCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => e.MetaFormCategoryId);
        }
    }
}