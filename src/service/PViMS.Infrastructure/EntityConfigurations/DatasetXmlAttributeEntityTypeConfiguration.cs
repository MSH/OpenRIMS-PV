using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class DatasetXmlAttributeEntityTypeConfiguration : IEntityTypeConfiguration<DatasetXmlAttribute>
    {
        public void Configure(EntityTypeBuilder<DatasetXmlAttribute> configuration)
        {
            configuration.ToTable("DatasetXmlAttribute");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.AttributeName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.DatasetElementId)
                .HasColumnName("DatasetElement_Id");

            configuration.Property(e => e.ParentNodeId)
                .IsRequired()
                .HasColumnName("ParentNode_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.DatasetXmlAttributeCreations)
                .HasForeignKey(d => d.CreatedById);

            configuration.HasOne(d => d.DatasetElement)
                .WithMany(p => p.DatasetXmlAttributes)
                .HasForeignKey(d => d.DatasetElementId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.ParentNode)
                .WithMany(p => p.NodeAttributes)
                .HasForeignKey(d => d.ParentNodeId)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.DatasetXmlAttributeUpdates)
                .HasForeignKey(d => d.UpdatedById);

            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.DatasetElementId);
            configuration.HasIndex(e => e.ParentNodeId);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
