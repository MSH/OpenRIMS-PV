using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class MetaColumnTypeEntityTypeConfiguration : IEntityTypeConfiguration<MetaColumnType>
    {
        public void Configure(EntityTypeBuilder<MetaColumnType> configuration)
        {
            configuration.ToTable("MetaColumnType");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.MetaColumnTypeGuid)
                .IsRequired()
                .HasColumnName("metacolumntype_guid");

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
