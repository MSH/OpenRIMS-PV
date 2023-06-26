using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Aggregates.ConceptAggregate;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> configuration)
        {
            configuration.ToTable("Product");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ConceptId)
                .IsRequired()
                .HasColumnName("Concept_Id");

            configuration.Property(e => e.Description)
                .HasMaxLength(1000);

            configuration.Property(e => e.Manufacturer)
                .IsRequired()
                .HasMaxLength(200);

            configuration.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            configuration.HasOne(d => d.Concept)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.ConceptId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => new { e.ConceptId, e.ProductName, e.Manufacturer }).IsUnique(true);
            configuration.HasIndex(e => e.ConceptId);
        }
    }
}
