using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class PatientMedicationEntityTypeConfiguration : IEntityTypeConfiguration<PatientMedication>
    {
        public void Configure(EntityTypeBuilder<PatientMedication> configuration)
        {
            configuration.ToTable("PatientMedication");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(e => e.ConceptId)
                .IsRequired()
                .HasColumnName("Concept_Id");

            configuration.Property(e => e.CustomAttributesXmlSerialised)
                .HasColumnType("xml");

            configuration.Property(e => e.EndDate)
                .HasColumnType("date");

            configuration.Property(e => e.StartDate)
                .IsRequired()
                .HasColumnType("date");

            configuration.Property(e => e.Dose)
                .HasMaxLength(30);

            configuration.Property(e => e.DoseFrequency)
                .HasMaxLength(30);

            configuration.Property(e => e.DoseUnit)
                .HasMaxLength(10);

            configuration.Property(e => e.MedicationSource)
                .HasMaxLength(200);

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(e => e.PatientMedicationGuid)
                .IsRequired()
                .HasDefaultValueSql("(newid())");

            configuration.Property(e => e.ProductId)
                .HasColumnName("Product_Id");

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.PatientMedications)
                .HasForeignKey(d => d.AuditUserId);

            configuration.HasOne(d => d.Concept)
                .WithMany(p => p.PatientMedications)
                .HasForeignKey(d => d.ConceptId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.PatientMedications)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.Product)
                .WithMany(p => p.PatientMedications)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasIndex(e => new { e.PatientId, e.ConceptId, e.ProductId }).IsUnique(false);
            configuration.HasIndex(e => e.AuditUserId);
            configuration.HasIndex(e => e.ConceptId);
            configuration.HasIndex(e => e.PatientId);
            configuration.HasIndex(e => e.ProductId);
        }
    }
}
