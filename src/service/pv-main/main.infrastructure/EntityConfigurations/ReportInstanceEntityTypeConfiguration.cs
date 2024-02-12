using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class ReportInstanceEntityTypeConfiguration : IEntityTypeConfiguration<ReportInstance>
    {
        public void Configure(EntityTypeBuilder<ReportInstance> configuration)
        {
            configuration.ToTable("ReportInstance");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.Identifier)
                .IsRequired();

            configuration.Property(e => e.PatientIdentifier)
                .IsRequired();

            configuration.Property(e => e.SourceIdentifier)
                .IsRequired();

            configuration.Property(e => e.TerminologyMedDraId)
                .HasColumnName("TerminologyMedDra_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(e => e.WorkFlowId)
                .IsRequired()
                .HasColumnName("WorkFlow_Id");

            configuration
                .Property(c => c.ReportClassificationId)
                .IsRequired();

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.ReportInstanceCreations)
                .HasForeignKey(d => d.CreatedById);

            configuration.HasOne(d => d.TerminologyMedDra)
                .WithMany(p => p.ReportInstances)
                .HasForeignKey(d => d.TerminologyMedDraId);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.ReportInstanceUpdates)
                .HasForeignKey(d => d.UpdatedById);

            configuration.HasOne(d => d.WorkFlow)
                .WithMany(p => p.ReportInstances)
                .HasForeignKey(d => d.WorkFlowId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.TerminologyMedDraId);
            configuration.HasIndex(e => e.UpdatedById);
            configuration.HasIndex(e => e.WorkFlowId);
        }
    }
}
