using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class CohortGroupEnrolmentEntityTypeConfiguration : IEntityTypeConfiguration<CohortGroupEnrolment>
    {
        public void Configure(EntityTypeBuilder<CohortGroupEnrolment> configuration)
        {
            configuration.ToTable("CohortGroupEnrolment");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(e => e.CohortGroupId)
                .IsRequired()
                .HasColumnName("CohortGroup_Id");

            configuration.Property(c => c.EnroledDate)
                .IsRequired();

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(c => c.Archived)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.CohortGroupEnrolments)
                .HasForeignKey(d => d.AuditUserId);

            configuration.HasOne(d => d.CohortGroup)
                .WithMany(p => p.CohortGroupEnrolments)
                .HasForeignKey(d => d.CohortGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.CohortEnrolments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => e.AuditUserId);
            configuration.HasIndex(e => e.CohortGroupId);
            configuration.HasIndex(e => e.PatientId);
        }
    }
}
