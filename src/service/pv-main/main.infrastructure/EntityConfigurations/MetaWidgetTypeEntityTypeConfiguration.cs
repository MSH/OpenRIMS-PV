using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class MetaWidgetTypeEntityTypeConfiguration : IEntityTypeConfiguration<MetaWidgetType>
    {
        public void Configure(EntityTypeBuilder<MetaWidgetType> configuration)
        {
            configuration.ToTable("MetaWidgetType");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.MetaWidgetTypeGuid)
                .IsRequired()
                .HasColumnName("metawidgettype_guid");

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
