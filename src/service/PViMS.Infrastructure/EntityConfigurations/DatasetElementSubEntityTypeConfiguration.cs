using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetElementSubEntityTypeConfiguration : IEntityTypeConfiguration<DatasetElementSub>
    {
        public void Configure(EntityTypeBuilder<DatasetElementSub> configuration)
        {
            configuration.ToTable("DatasetElementSub");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.DatasetElementId)
                .IsRequired()
                .HasColumnName("DatasetElement_Id");

            configuration.Property(c => c.ElementName)
                .IsRequired()
                .HasMaxLength(100);

            configuration.Property(e => e.FieldId)
                .IsRequired()
                .HasColumnName("Field_Id");

            configuration.Property(c => c.FriendlyName)
                .HasMaxLength(150);

            configuration.Property(c => c.Help)
                .HasMaxLength(350);

            configuration.Property(c => c.Oid)
                .HasColumnName("OID")
                .HasMaxLength(50);

            configuration.Property(c => c.FieldOrder)
                .IsRequired();

            configuration.Property(c => c.System)
                .IsRequired();

            configuration.HasOne(d => d.DatasetElement)
                .WithMany(p => p.DatasetElementSubs)
                .HasForeignKey(d => d.DatasetElementId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.Field)
                .WithMany(p => p.DatasetElementSubs)
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasIndex(e => new { e.DatasetElementId, e.ElementName }).IsUnique(true);
            configuration.HasIndex(e => e.DatasetElementId);
            configuration.HasIndex(e => e.FieldId);
        }
    }
}
