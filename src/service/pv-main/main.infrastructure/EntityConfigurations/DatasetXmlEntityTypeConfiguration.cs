using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class DatasetXmlEntityTypeConfiguration : IEntityTypeConfiguration<DatasetXml>
    {
        public void Configure(EntityTypeBuilder<DatasetXml> configuration)
        {
            configuration.ToTable("DatasetXml");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.DatasetXmlCreations)
                .HasForeignKey(d => d.CreatedById);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.DatasetXmlUpdates)
                .HasForeignKey(d => d.UpdatedById);

            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
