using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class PatientStatusHistoryEntityTypeConfiguration : IEntityTypeConfiguration<PatientStatusHistory>
    {
        public void Configure(EntityTypeBuilder<PatientStatusHistory> configuration)
        {
            configuration.ToTable("PatientStatusHistory");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(e => e.Comments)
                .HasMaxLength(100);

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.EffectiveDate)
                .IsRequired();

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(e => e.PatientStatusId)
                .IsRequired()
                .HasColumnName("PatientStatus_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.PatientStatusHistories)
                .HasForeignKey(d => d.AuditUserId);

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.PatientStatusHistoryCreations)
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.PatientStatusHistories)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.PatientStatus)
                .WithMany(p => p.PatientStatusHistories)
                .HasForeignKey(d => d.PatientStatusId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.PatientStatusHistoryUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasIndex(e => new { e.PatientId, e.PatientStatusId }).IsUnique(false);
            configuration.HasIndex(e => e.AuditUserId);
            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.PatientStatusId);
            configuration.HasIndex(e => e.PatientId);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
