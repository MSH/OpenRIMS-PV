using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class PatientLanguageEntityTypeConfiguration : IEntityTypeConfiguration<PatientLanguage>
    {
        public void Configure(EntityTypeBuilder<PatientLanguage> configuration)
        {
            configuration.ToTable("PatientLanguage");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnName("Patient_Id");

            configuration.Property(e => e.LanguageId)
                .IsRequired()
                .HasColumnName("Language_Id");

            configuration.HasOne(d => d.Language)
                .WithMany(p => p.PatientLanguages)
                .HasForeignKey(d => d.LanguageId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.Patient)
                .WithMany(p => p.PatientLanguages)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => new { e.PatientId, e.LanguageId }).IsUnique(true);
            configuration.HasIndex(e => e.LanguageId);
            configuration.HasIndex(e => e.PatientId);
        }
    }
}
