using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class FieldValueEntityTypeConfiguration : IEntityTypeConfiguration<FieldValue>
    {
        public void Configure(EntityTypeBuilder<FieldValue> configuration)
        {
            configuration.ToTable("FieldValue");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Value)
                .IsRequired()
                .HasMaxLength(100);

            configuration.Property(e => e.FieldId)
                .IsRequired()
                .HasColumnName("Field_Id");

            configuration.Property(c => c.Default)
                .IsRequired();

            configuration.Property(c => c.Other)
                .IsRequired();

            configuration.Property(c => c.Unknown)
                .IsRequired();

            configuration.HasOne(d => d.Field)
                .WithMany(p => p.FieldValues)
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => e.FieldId);
        }
    }
}
