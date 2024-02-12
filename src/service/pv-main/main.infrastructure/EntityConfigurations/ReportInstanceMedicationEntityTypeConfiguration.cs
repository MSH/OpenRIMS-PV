using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class ReportInstanceMedicationEntityTypeConfiguration : IEntityTypeConfiguration<ReportInstanceMedication>
    {
        public void Configure(EntityTypeBuilder<ReportInstanceMedication> configuration)
        {
            configuration.ToTable("ReportInstanceMedication");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.NaranjoCausality)
                .HasMaxLength(30);

            configuration.Property(e => e.ReportInstanceId)
                .IsRequired()
                .HasColumnName("ReportInstance_Id");

            configuration.Property(e => e.WhoCausality)
                .HasMaxLength(30);

            configuration.Property(e => e.ReportInstanceMedicationGuid)
                .IsRequired()
                .HasMaxLength(30);

            configuration.HasOne(d => d.ReportInstance)
                .WithMany(p => p.Medications)
                .HasForeignKey(d => d.ReportInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => e.ReportInstanceId);
        }
    }
}
