using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class TerminologyMedDraEntityTypeConfiguration : IEntityTypeConfiguration<TerminologyMedDra>
    {
        public void Configure(EntityTypeBuilder<TerminologyMedDra> configuration)
        {
            configuration.ToTable("TerminologyMedDra");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.MedDraCode)
                .IsRequired()
                .HasMaxLength(10);

            configuration.Property(e => e.MedDraTerm)
                .IsRequired()
                .HasMaxLength(100);

            configuration.Property(e => e.MedDraTermType)
                .IsRequired()
                .HasMaxLength(4);

            configuration.Property(e => e.MedDraVersion)
                .HasMaxLength(7);

            configuration.Property(e => e.ParentId)
                .HasColumnName("Parent_Id");

            configuration.HasOne(d => d.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasIndex(e => e.ParentId);
        }
    }
}
