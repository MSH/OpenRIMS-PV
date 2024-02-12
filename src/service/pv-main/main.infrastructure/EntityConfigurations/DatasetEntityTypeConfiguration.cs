using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class DatasetEntityTypeConfiguration : IEntityTypeConfiguration<Dataset>
    {
        public void Configure(EntityTypeBuilder<Dataset> configuration)
        {
            configuration.ToTable("Dataset");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ContextTypeId)
                .IsRequired()
                .HasColumnName("ContextType_Id");

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(c => c.DatasetName)
                .IsRequired(true)
                .HasMaxLength(50);

            configuration.Property(e => e.DatasetXmlId)
                .HasColumnName("DatasetXml_Id");

            configuration.Property(e => e.EncounterTypeWorkPlanId)
                .HasColumnName("EncounterTypeWorkPlan_Id");

            configuration.Property(c => c.Help)
                .HasMaxLength(250);

            configuration.Property(c => c.InitialiseProcess)
                .HasMaxLength(100);

            configuration.Property(c => c.RulesProcess)
                .HasMaxLength(100);

            configuration.Property(c => c.Uid)
                .HasColumnName("UID")
                .HasMaxLength(10);

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(c => c.Active)
                .IsRequired();

            configuration.Property(c => c.IsSystem)
                .IsRequired();

            configuration.HasOne(d => d.ContextType)
                .WithMany(p => p.Datasets)
                .HasForeignKey(d => d.ContextTypeId);

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.DatasetCreations)
                .HasForeignKey(d => d.CreatedById);

            configuration.HasOne(d => d.DatasetXml)
                .WithMany(p => p.Datasets)
                .HasForeignKey(d => d.DatasetXmlId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasOne(d => d.EncounterTypeWorkPlan)
                .WithMany(p => p.Datasets)
                .HasForeignKey(d => d.EncounterTypeWorkPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.DatasetUpdates)
                .HasForeignKey(d => d.UpdatedById);

            configuration.HasIndex("DatasetName").IsUnique(true);
            configuration.HasIndex(e => e.ContextTypeId);
            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.DatasetXmlId);
            configuration.HasIndex(e => e.EncounterTypeWorkPlanId);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
