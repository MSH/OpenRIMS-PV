using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class MetaWidgetEntityTypeConfiguration : IEntityTypeConfiguration<MetaWidget>
    {
        public void Configure(EntityTypeBuilder<MetaWidget> configuration)
        {
            configuration.ToTable("MetaWidget");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.MetaPageId)
                .IsRequired()
                .HasColumnName("MetaPage_Id");

            configuration.Property(e => e.MetaWidgetGuid)
                .IsRequired()
                .HasColumnName("metawidget_guid");

            configuration.Property(e => e.WidgetDefinition)
                .HasMaxLength(250);

            configuration.Property(e => e.WidgetName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.WidgetTypeId)
                .IsRequired()
                .HasColumnName("WidgetType_Id");

            configuration.Property(c => c.WidgetLocation)
                .HasConversion(x => (int)x, x => (MetaWidgetLocation)x);

            configuration.Property(c => c.WidgetStatus)
                .HasConversion(x => (int)x, x => (MetaWidgetStatus)x);

            configuration.HasOne(d => d.MetaPage)
                .WithMany(p => p.Widgets)
                .HasForeignKey(d => d.MetaPageId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.WidgetType)
                .WithMany(p => p.MetaWidgets)
                .HasForeignKey(d => d.WidgetTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasIndex("WidgetName").IsUnique(true);
            configuration.HasIndex(e => e.MetaPageId);
            configuration.HasIndex(e => e.WidgetTypeId);
        }
    }
}
