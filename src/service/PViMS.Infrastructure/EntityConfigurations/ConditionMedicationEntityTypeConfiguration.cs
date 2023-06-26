using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ConditionMedicationEntityTypeConfiguration : IEntityTypeConfiguration<ConditionMedication>
    {
        public void Configure(EntityTypeBuilder<ConditionMedication> configuration)
        {
            configuration.ToTable("ConditionMedication");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ConceptId)
                .IsRequired()
                .HasColumnName("Concept_Id");

            configuration.Property(e => e.ConditionId)
                .HasColumnName("Condition_Id");

            configuration.Property(e => e.ProductId)
                .HasColumnName("Product_Id");

            configuration.HasOne(d => d.Concept)
                .WithMany(p => p.ConditionMedications)
                .HasForeignKey(d => d.ConceptId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.Condition)
                .WithMany(p => p.ConditionMedications)
                .HasForeignKey(d => d.ConditionId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.Product)
                .WithMany(p => p.ConditionMedications)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasIndex(e => new { e.ConditionId, e.ConceptId, e.ProductId }).IsUnique(true);
            configuration.HasIndex(e => e.ConceptId);
            configuration.HasIndex(e => e.ConditionId);
            configuration.HasIndex(e => e.ProductId);
        }
    }
}
