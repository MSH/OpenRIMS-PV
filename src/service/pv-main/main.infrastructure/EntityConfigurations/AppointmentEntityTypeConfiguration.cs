using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class AppointmentEntityTypeConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> configuration)
        {
            configuration.ToTable("Appointment");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.AppointmentDate)
                .IsRequired();

            configuration.Property(e => e.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(c => c.CancellationReason)
                .HasMaxLength(250);

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.Dna)
                .IsRequired()
                .HasColumnName("DNA")
                .HasDefaultValue(false);

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(c => c.Reason)
                .IsRequired()
                .HasMaxLength(250);

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(c => c.Cancelled)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(c => c.Archived)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(c => c.ArchivedReason)
                .HasMaxLength(200);

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.Appointments)
                .HasForeignKey(d => d.AuditUserId);

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.AppointmentCreations)
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.AppointmentUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasIndex("AppointmentDate").IsUnique(false);
            configuration.HasIndex(e => e.AuditUserId);
            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.PatientId);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
