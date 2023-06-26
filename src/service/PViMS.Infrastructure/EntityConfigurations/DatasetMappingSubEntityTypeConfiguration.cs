using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetMappingSubEntityTypeConfiguration : IEntityTypeConfiguration<DatasetMappingSub>
    {
        public void Configure(EntityTypeBuilder<DatasetMappingSub> configuration)
        {
            configuration.ToTable("DatasetMappingSub");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.DestinationElementId)
                .IsRequired()
                .HasColumnName("DestinationElement_Id");

            configuration.Property(e => e.MappingId)
                .IsRequired()
                .HasColumnName("Mapping_Id");

            configuration.Property(e => e.SourceElementId)
                .HasColumnName("SourceElement_Id");

            configuration.Property(c => c.MappingType)
                .HasConversion(x => (int)x, x => (MappingType)x);

            configuration.HasOne(d => d.DestinationElement)
                .WithMany(p => p.DatasetMappingSubDestinationElements)
                .HasForeignKey(d => d.DestinationElementId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.Mapping)
                .WithMany(p => p.SubMappings)
                .HasForeignKey(d => d.MappingId)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.SourceElement)
                .WithMany(p => p.DatasetMappingSubSourceElements)
                .HasForeignKey(d => d.SourceElementId)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasIndex(e => e.DestinationElementId);
            configuration.HasIndex(e => e.MappingId);
            configuration.HasIndex(e => e.SourceElementId);
        }
    }
}
