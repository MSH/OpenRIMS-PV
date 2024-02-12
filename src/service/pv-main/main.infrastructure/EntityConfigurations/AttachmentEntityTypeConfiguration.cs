using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class AttachmentEntityTypeConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> configuration)
        {
            configuration.ToTable("Attachment");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ActivityExecutionStatusEventId)
                .HasColumnName("ActivityExecutionStatusEvent_Id");

            configuration.Property(c => c.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AttachmentTypeId)
                .IsRequired()
                .HasColumnName("AttachmentType_Id");

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(c => c.Content)
                .IsRequired();

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(c => c.Description)
                .HasMaxLength(100);

            configuration.Property(e => e.EncounterId)
                .HasColumnName("Encounter_Id");

            configuration.Property(c => c.FileName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.PatientId)
                .HasColumnName("Patient_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(c => c.Size)
                .IsRequired(true);

            configuration.Property(c => c.Archived)
                .HasDefaultValue(false)
                .IsRequired();

            configuration.HasOne(d => d.ActivityExecutionStatusEvent)
                .WithMany(p => p.Attachments)
                .HasForeignKey(d => d.ActivityExecutionStatusEventId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.AttachmentType)
                .WithMany(p => p.Attachments)
                .HasForeignKey(d => d.AttachmentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.Attachments)
                .HasForeignKey(d => d.AuditUserId);

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.AttachmentCreations)
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.Encounter)
                .WithMany(p => p.Attachments)
                .HasForeignKey(d => d.EncounterId)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.Attachments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.AttachmentUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasIndex(e => e.ActivityExecutionStatusEventId);
            configuration.HasIndex(e => e.AttachmentTypeId);
            configuration.HasIndex(e => e.AuditUserId);
            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.EncounterId);
            configuration.HasIndex(e => e.PatientId);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
