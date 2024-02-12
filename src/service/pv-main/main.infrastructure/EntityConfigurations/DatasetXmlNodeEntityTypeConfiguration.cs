using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class DatasetXmlNodeEntityTypeConfiguration : IEntityTypeConfiguration<DatasetXmlNode>
    {
        public void Configure(EntityTypeBuilder<DatasetXmlNode> configuration)
        {
            configuration.ToTable("DatasetXmlNode");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.DatasetElementId)
                .HasColumnName("DatasetElement_Id");

            configuration.Property(e => e.DatasetElementSubId)
                .HasColumnName("DatasetElementSub_Id");

            configuration.Property(e => e.DatasetXmlId)
                .IsRequired()
                .HasColumnName("DatasetXml_Id");

            configuration.Property(c => c.NodeName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(c => c.NodeType)
                .HasConversion(x => (int)x, x => (NodeType)x);

            configuration.Property(e => e.ParentNodeId)
                .HasColumnName("ParentNode_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.DatasetXmlNodeCreations)
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.DatasetElement)
                .WithMany(p => p.DatasetXmlNodes)
                .HasForeignKey(d => d.DatasetElementId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.DatasetElementSub)
                .WithMany(p => p.DatasetXmlNodes)
                .HasForeignKey(d => d.DatasetElementSubId)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.DatasetXml)
                .WithMany(p => p.ChildrenNodes)
                .HasForeignKey(d => d.DatasetXmlId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.ParentNode)
                .WithMany(p => p.ChildrenNodes)
                .HasForeignKey(d => d.ParentNodeId)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.DatasetXmlNodeUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasIndex(e => new { e.DatasetXmlId, e.NodeName }).IsUnique(true);
            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.DatasetElementSubId);
            configuration.HasIndex(e => e.DatasetElementId);
            configuration.HasIndex(e => e.DatasetXmlId);
            configuration.HasIndex(e => e.ParentNodeId);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
