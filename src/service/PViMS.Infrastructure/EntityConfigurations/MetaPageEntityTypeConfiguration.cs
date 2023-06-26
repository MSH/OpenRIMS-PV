using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class MetaPageEntityTypeConfiguration : IEntityTypeConfiguration<MetaPage>
    {
        public void Configure(EntityTypeBuilder<MetaPage> configuration)
        {
            configuration.ToTable("MetaPage");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Breadcrumb)
                .HasMaxLength(250);

            configuration.Property(e => e.MetaPageGuid)
                .IsRequired()
                .HasColumnName("metapage_guid");

            configuration.Property(e => e.PageDefinition)
                .HasMaxLength(250);

            configuration.Property(e => e.PageName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.IsSystem)
                .IsRequired();

            configuration.Property(e => e.IsVisible)
                .IsRequired();

            configuration.HasIndex("PageName").IsUnique(true);
        }
    }
}
