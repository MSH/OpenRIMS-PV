using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetMappingEntityTypeConfiguration : IEntityTypeConfiguration<DatasetMapping>
    {
        public void Configure(EntityTypeBuilder<DatasetMapping> configuration)
        {
            configuration.ToTable("DatasetMapping");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.DestinationElementId)
                .IsRequired()
                .HasColumnName("DestinationElement_Id");

            configuration.Property(e => e.SourceElementId)
                .HasColumnName("SourceElement_Id");

            configuration.Property(c => c.MappingType)
                .HasConversion(x => (int)x, x => (MappingType)x);

            configuration.HasOne(d => d.DestinationElement)
                .WithMany(p => p.DestinationMappings)
                .HasForeignKey(d => d.DestinationElementId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.SourceElement)
                .WithMany(p => p.SourceMappings)
                .HasForeignKey(d => d.SourceElementId)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasIndex(e => e.DestinationElementId);
            configuration.HasIndex(e => e.SourceElementId);
        }
    }
}
