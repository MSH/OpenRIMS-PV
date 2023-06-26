using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class MedDRAGradingEntityTypeConfiguration : IEntityTypeConfiguration<MedDRAGrading>
    {
        public void Configure(EntityTypeBuilder<MedDRAGrading> configuration)
        {
            configuration.ToTable("MedDRAGrading");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(100);

            configuration.Property(c => c.Grade)
                .IsRequired()
                .HasMaxLength(20);

            configuration.Property(e => e.ScaleId)
                .IsRequired()
                .HasColumnName("Scale_Id");

            configuration.HasOne(d => d.Scale)
                .WithMany(p => p.Grades)
                .HasForeignKey(d => d.ScaleId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => e.ScaleId);
        }
    }
}
