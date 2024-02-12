using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class PatientLabTestEntityTypeConfiguration : IEntityTypeConfiguration<PatientLabTest>
    {
        public void Configure(EntityTypeBuilder<PatientLabTest> configuration)
        {
            configuration.ToTable("PatientLabTest");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(e => e.CustomAttributesXmlSerialised)
                .HasColumnType("xml");

            configuration.Property(e => e.LabTestId)
                .IsRequired()
                .HasColumnName("LabTest_Id");

            configuration.Property(e => e.LabTestSource)
                .HasMaxLength(200);

            configuration.Property(e => e.LabValue)
                .HasMaxLength(20);

            configuration.Property(e => e.PatientLabTestGuid)
                .IsRequired()
                .HasDefaultValueSql("(newid())");

            configuration.Property(e => e.ReferenceLower)
                .HasMaxLength(20);

            configuration.Property(e => e.ReferenceUpper)
                .HasMaxLength(20);

            configuration.Property(e => e.TestDate)
                .IsRequired()
                .HasColumnType("date");

            configuration.Property(e => e.TestResult)
                .HasMaxLength(50);

            configuration.Property(e => e.TestUnitId)
                .HasColumnName("TestUnit_Id");

            configuration.Property(c => c.Archived)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.PatientLabTests)
                .HasForeignKey(d => d.AuditUserId);

            configuration.HasOne(d => d.LabTest)
                .WithMany(p => p.PatientLabTests)
                .HasForeignKey(d => d.LabTestId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.PatientLabTests)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.TestUnit)
                .WithMany(p => p.PatientLabTests)
                .HasForeignKey(d => d.TestUnitId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => new { e.PatientId, e.LabTestId }).IsUnique(false);
            configuration.HasIndex(e => e.AuditUserId);
            configuration.HasIndex(e => e.LabTestId);
            configuration.HasIndex(e => e.PatientId);
            configuration.HasIndex(e => e.TestUnitId);
        }
    }
}
