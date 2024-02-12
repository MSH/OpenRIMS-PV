using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class FieldEntityTypeConfiguration : IEntityTypeConfiguration<Field>
    {
        public void Configure(EntityTypeBuilder<Field> configuration)
        {
            configuration.ToTable("Field");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Calculation)
                .HasMaxLength(100);

            configuration.Property(e => e.FieldTypeId)
                .IsRequired()
                .HasColumnName("FieldType_Id");

            configuration.Property(c => c.FileExt)
                .HasMaxLength(100);

            configuration.Property(c => c.Image)
                .HasColumnType("image");

            configuration.Property(c => c.MaxSize)
                .HasColumnType("decimal(18, 2)");

            configuration.Property(c => c.MinSize)
                .HasColumnType("decimal(18, 2)");

            configuration.Property(c => c.RegEx)
                .HasMaxLength(100);

            configuration.Property(c => c.Mandatory)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(c => c.Anonymise)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasOne(d => d.FieldType)
                .WithMany(p => p.Fields)
                .HasForeignKey(d => d.FieldTypeId);

            configuration.HasIndex(e => e.FieldTypeId);
        }
    }
}
