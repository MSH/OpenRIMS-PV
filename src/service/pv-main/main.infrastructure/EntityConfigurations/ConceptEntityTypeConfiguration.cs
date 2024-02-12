using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Aggregates.ConceptAggregate;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class ConceptEntityTypeConfiguration : IEntityTypeConfiguration<Concept>
    {
        public void Configure(EntityTypeBuilder<Concept> configuration)
        {
            configuration.ToTable("Concept");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ConceptName)
                .IsRequired()
                .HasMaxLength(1000);

            configuration.Property(c => c.Active)
                .IsRequired();

            configuration.Property(c => c.Strength)
                .HasMaxLength(250);

            configuration.Property(e => e.MedicationFormId)
                .IsRequired()
                .HasColumnName("MedicationForm_Id");

            configuration.HasOne(d => d.MedicationForm)
                .WithMany(p => p.Concepts)
                .HasForeignKey(d => d.MedicationFormId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasIndex(e => new { e.ConceptName, e.Strength, e.MedicationFormId }).IsUnique(true);
            configuration.HasIndex(e => e.MedicationFormId);
        }
    }
}
