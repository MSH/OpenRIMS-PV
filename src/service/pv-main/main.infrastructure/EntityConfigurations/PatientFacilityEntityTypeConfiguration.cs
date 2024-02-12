using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class PatientFacilityEntityTypeConfiguration : IEntityTypeConfiguration<PatientFacility>
    {
        public void Configure(EntityTypeBuilder<PatientFacility> configuration)
        {
            configuration.ToTable("PatientFacility");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(e => e.EnrolledDate)
                .IsRequired();

            configuration.Property(e => e.FacilityId)
                .IsRequired()
                .HasColumnName("Facility_Id");

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(c => c.Archived)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.PatientFacilities)
                .HasForeignKey(d => d.AuditUserId);

            configuration.HasOne(d => d.Facility)
                .WithMany(p => p.PatientFacilities)
                .HasForeignKey(d => d.FacilityId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.PatientFacilities)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => new { e.PatientId, e.FacilityId }).IsUnique(false);
            configuration.HasIndex(e => e.AuditUserId);
            configuration.HasIndex(e => e.FacilityId);
            configuration.HasIndex(e => e.PatientId);
        }
    }
}
