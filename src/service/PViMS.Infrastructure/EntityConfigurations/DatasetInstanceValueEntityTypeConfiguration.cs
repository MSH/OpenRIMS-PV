using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetInstanceValueEntityTypeConfiguration : IEntityTypeConfiguration<DatasetInstanceValue>
    {
        public void Configure(EntityTypeBuilder<DatasetInstanceValue> configuration)
        {
            configuration.ToTable("DatasetInstanceValue");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.DatasetElementId)
                .IsRequired()
                .HasColumnName("DatasetElement_Id");

            configuration.Property(e => e.DatasetInstanceId)
                .IsRequired()
                .HasColumnName("DatasetInstance_Id");

            configuration.Property(c => c.InstanceValue)
                .IsRequired();

            configuration.HasOne(d => d.DatasetElement)
                .WithMany(p => p.DatasetInstanceValues)
                .HasForeignKey(d => d.DatasetElementId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.DatasetInstance)
                .WithMany(p => p.DatasetInstanceValues)
                .HasForeignKey(d => d.DatasetInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => new { e.DatasetInstanceId, e.DatasetElementId }).IsUnique(true);
            configuration.HasIndex(e => e.DatasetElementId);
            configuration.HasIndex(e => e.DatasetInstanceId);
        }
    }
}
