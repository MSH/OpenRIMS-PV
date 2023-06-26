using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class PatientConditionEntityTypeConfiguration : IEntityTypeConfiguration<PatientCondition>
    {
        public void Configure(EntityTypeBuilder<PatientCondition> configuration)
        {
            configuration.ToTable("PatientCondition");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(e => e.Comments)
                .HasMaxLength(500);

            configuration.Property(e => e.ConditionId)
                .HasColumnName("Condition_Id");

            configuration.Property(e => e.ConditionSource)
                .HasMaxLength(200);

            configuration.Property(e => e.CaseNumber)
                .HasMaxLength(50);

            configuration.Property(e => e.CustomAttributesXmlSerialised)
                .HasColumnType("xml");

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(e => e.OnsetDate)
                .IsRequired()
                .HasColumnType("date");

            configuration.Property(e => e.OutcomeDate)
                .HasColumnType("date");

            configuration.Property(e => e.OutcomeId)
                .HasColumnName("Outcome_Id");

            configuration.Property(e => e.PatientConditionGuid)
                .IsRequired()
                .HasDefaultValueSql("(newid())");

            configuration.Property(e => e.TerminologyMedDraId)
                .HasColumnName("TerminologyMedDra_Id");

            configuration.Property(e => e.TreatmentOutcomeId)
                .HasColumnName("TreatmentOutcome_Id");

            configuration.Property(c => c.Archived)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.PatientConditions)
                .HasForeignKey(d => d.AuditUserId);

            configuration.HasOne(d => d.Condition)
                .WithMany(p => p.PatientConditions)
                .HasForeignKey(d => d.ConditionId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.Outcome)
                .WithMany(p => p.PatientConditions)
                .HasForeignKey(d => d.OutcomeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.PatientConditions)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.TerminologyMedDra)
                .WithMany(p => p.PatientConditions)
                .HasForeignKey(d => d.TerminologyMedDraId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasOne(d => d.TreatmentOutcome)
                .WithMany(p => p.PatientConditions)
                .HasForeignKey(d => d.TreatmentOutcomeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasIndex(e => e.AuditUserId);
            configuration.HasIndex(e => e.ConditionId);
            configuration.HasIndex(e => e.OutcomeId);
            configuration.HasIndex(e => e.PatientId);
            configuration.HasIndex(e => e.TerminologyMedDraId);
            configuration.HasIndex(e => e.TreatmentOutcomeId);
        }
    }
}
