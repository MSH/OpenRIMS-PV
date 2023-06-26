using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class EncounterEntityTypeConfiguration : IEntityTypeConfiguration<Encounter>
    {
        public void Configure(EntityTypeBuilder<Encounter> configuration)
        {
            configuration.ToTable("Encounter");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ArchivedReason)
                .HasMaxLength(200);

            configuration.Property(e => e.AuditUserId)
                .HasColumnName("AuditUser_Id");

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(c => c.EncounterDate)
                .IsRequired()
                .HasColumnType("date");

            configuration.Property(e => e.EncounterTypeId)
                .IsRequired()
                .HasColumnName("EncounterType_Id");

            configuration.Property(c => c.EncounterGuid)
                .IsRequired()
                .HasDefaultValueSql("newid()");

            configuration.Property(c => c.Discharged)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(e => e.PriorityId)
                .IsRequired()
                .HasColumnName("Priority_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.Encounters)
                .HasForeignKey(d => d.AuditUserId);

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.EncounterCreations)
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.EncounterType)
                .WithMany(p => p.Encounters)
                .HasForeignKey(d => d.EncounterTypeId);

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.Encounters)
                .HasForeignKey(d => d.PatientId);

            configuration.HasOne(d => d.Priority)
                .WithMany(p => p.Encounters)
                .HasForeignKey(d => d.PriorityId);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.EncounterUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.Property(c => c.Archived)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasIndex(e => e.EncounterDate).IsUnique(false);
            configuration.HasIndex(e => new { e.PatientId, e.EncounterDate }).IsUnique(false);
            configuration.HasIndex(e => e.AuditUserId);
            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.EncounterTypeId);
            configuration.HasIndex(e => e.PatientId);
            configuration.HasIndex(e => e.PriorityId);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
