using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class ConditionLabTestEntityTypeConfiguration : IEntityTypeConfiguration<ConditionLabTest>
    {
        public void Configure(EntityTypeBuilder<ConditionLabTest> configuration)
        {
            configuration.ToTable("ConditionLabTest");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ConditionId)
                .IsRequired()
                .HasColumnName("Condition_Id");

            configuration.Property(e => e.LabTestId)
                .IsRequired()
                .HasColumnName("LabTest_Id");

            configuration.HasOne(d => d.Condition)
                .WithMany(p => p.ConditionLabTests)
                .HasForeignKey(d => d.ConditionId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.LabTest)
                .WithMany(p => p.ConditionLabTests)
                .HasForeignKey(d => d.LabTestId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => new { e.ConditionId, e.LabTestId }).IsUnique(true);
            configuration.HasIndex(e => e.ConditionId);
            configuration.HasIndex(e => e.LabTestId);
        }
    }
}
