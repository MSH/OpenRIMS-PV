using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetElementEntityTypeConfiguration : IEntityTypeConfiguration<DatasetElement>
    {
        public void Configure(EntityTypeBuilder<DatasetElement> configuration)
        {
            configuration.ToTable("DatasetElement");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.DatasetElementTypeId)
                .IsRequired()
                .HasColumnName("DatasetElementType_Id");

            configuration.Property(c => c.ElementName)
                .IsRequired()
                .HasMaxLength(100);

            configuration.Property(e => e.FieldId)
                .IsRequired()
                .HasColumnName("Field_Id");

            configuration.Property(c => c.Oid)
                .HasColumnName("OID")
                .HasMaxLength(50);

            configuration.Property(c => c.Uid)
                .HasColumnName("UID")
                .HasMaxLength(10);

            configuration.Property(c => c.System)
                .IsRequired();

            configuration.Property(c => c.DatasetElementGuid)
                .IsRequired()
                .HasDefaultValueSql("newid()");

            configuration.HasOne(d => d.DatasetElementType)
                .WithMany(p => p.DatasetElements)
                .HasForeignKey(d => d.DatasetElementTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasOne(d => d.Field)
                .WithMany(p => p.DatasetElements)
                .HasForeignKey(d => d.FieldId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasIndex("ElementName").IsUnique(true);
            configuration.HasIndex(e => e.DatasetElementTypeId);
            configuration.HasIndex(e => e.FieldId);
        }
    }
}
