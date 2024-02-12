using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class MetaDependencyEntityTypeConfiguration : IEntityTypeConfiguration<MetaDependency>
    {
        public void Configure(EntityTypeBuilder<MetaDependency> configuration)
        {
            configuration.ToTable("MetaDependency");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.MetaDependencyGuid)
                .IsRequired()
                .HasColumnName("metadependency_guid");

            configuration.Property(e => e.ParentColumnName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.ParentTableId)
                .IsRequired()
                .HasColumnName("ParentTable_Id");

            configuration.Property(e => e.ReferenceColumnName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.ReferenceTableId)
                .IsRequired()
                .HasColumnName("ReferenceTable_Id");

            configuration.HasOne(d => d.ParentTable)
                .WithMany(p => p.MetaDependencyParentTables)
                .HasForeignKey(d => d.ParentTableId)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.ReferenceTable)
                .WithMany(p => p.MetaDependencyReferenceTables)
                .HasForeignKey(d => d.ReferenceTableId)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasIndex(e => e.ParentTableId);
            configuration.HasIndex(e => e.ReferenceTableId);
        }
    }
}
