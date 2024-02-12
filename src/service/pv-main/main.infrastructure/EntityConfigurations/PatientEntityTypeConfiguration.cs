using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class PatientEntityTypeConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> configuration)
        {
            configuration.ToTable("Patient");

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

            configuration.Property(e => e.CustomAttributesXmlSerialised)
                .HasColumnType("xml");

            configuration.Property(e => e.DateOfBirth)
                .HasColumnType("date");

            configuration.Property(e => e.FirstName)
                .IsRequired();

            configuration.Property(e => e.Surname)
                .IsRequired();

            configuration.Property(c => c.PatientGuid)
                .IsRequired()
                .HasDefaultValueSql("newid()");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(c => c.Archived)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.HasOne(d => d.AuditUser)
                .WithMany(p => p.Patients)
                .HasForeignKey(d => d.AuditUserId);

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.PatientCreations)
                .HasForeignKey(d => d.CreatedById);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.PatientUpdates)
                .HasForeignKey(d => d.UpdatedById);

            configuration.HasIndex(new string[] { "Surname", "FirstName" }).IsUnique(false);
            configuration.HasIndex(e => e.AuditUserId);
            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
