using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class FieldTypeEntityTypeConfiguration : IEntityTypeConfiguration<FieldType>
    {
        public void Configure(EntityTypeBuilder<FieldType> configuration)
        {
            configuration.ToTable("FieldType");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
