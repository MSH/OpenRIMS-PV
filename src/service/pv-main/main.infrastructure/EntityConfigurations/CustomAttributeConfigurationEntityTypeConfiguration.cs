using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.CustomAttributes;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class CustomAttributeConfigurationEntityTypeConfiguration : IEntityTypeConfiguration<CustomAttributeConfiguration>
    {
        public void Configure(EntityTypeBuilder<CustomAttributeConfiguration> configuration)
        {
            configuration.ToTable("CustomAttributeConfiguration");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.AttributeDetail)
                .HasMaxLength(150);

            configuration.Property(c => c.ExtendableTypeName)
                .IsRequired();

            configuration.Property(c => c.CustomAttributeType)
                .HasConversion(x => (int)x, x => (CustomAttributeType)x);

            configuration.Property(c => c.IsRequired)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(c => c.FutureDateOnly)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(c => c.PastDateOnly)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(c => c.IsSearchable)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasIndex(new string[] { "ExtendableTypeName", "CustomAttributeType", "AttributeKey" }).IsUnique(true);
        }
    }
}
