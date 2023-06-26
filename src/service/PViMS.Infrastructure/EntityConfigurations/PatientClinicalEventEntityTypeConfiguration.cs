using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class PatientClinicalEventEntityTypeConfiguration : IEntityTypeConfiguration<PatientClinicalEvent>
    {
        public void Configure(EntityTypeBuilder<PatientClinicalEvent> configuration)
        {
            configuration.ToTable("PatientClinicalEvent");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(e => e.CustomAttributesXmlSerialised)
                .HasColumnType("xml");

            configuration.Property(e => e.EncounterId)
                .HasColumnName("Encounter_Id");

            configuration.Property(e => e.OnsetDate)
                .HasColumnType("date");

            configuration.Property(e => e.PatientClinicalEventGuid)
                .IsRequired()
                .HasDefaultValueSql("(newid())");

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(e => e.ResolutionDate)
                .HasColumnType("date");

            configuration.Property(e => e.SourceDescription)
                .HasMaxLength(500);

            configuration.Property(e => e.SourceTerminologyMedDraId)
                .HasColumnName("SourceTerminologyMedDra_Id");

            configuration.Property(e => e.TerminologyMedDraId1)
                .HasColumnName("TerminologyMedDra_Id1");

            configuration.Property(c => c.Archived)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.PatientClinicalEvents)
                .HasForeignKey(d => d.AuditUserId);

            configuration.HasOne(d => d.Encounter)
                .WithMany(p => p.PatientClinicalEvents)
                .HasForeignKey(d => d.EncounterId);

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.PatientClinicalEvents)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.SourceTerminologyMedDra)
                .WithMany(p => p.PatientClinicalEvents)
                .HasForeignKey(d => d.SourceTerminologyMedDraId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasIndex(e => e.AuditUserId);
            configuration.HasIndex(e => e.EncounterId);
            configuration.HasIndex(e => e.PatientId);
            configuration.HasIndex(e => e.SourceTerminologyMedDraId);
        }
    }
}
