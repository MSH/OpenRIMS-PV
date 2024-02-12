using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class CohortGroupEntityTypeConfiguration : IEntityTypeConfiguration<CohortGroup>
    {
        public void Configure(EntityTypeBuilder<CohortGroup> configuration)
        {
            configuration.ToTable("CohortGroup");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.CohortCode)
                .IsRequired()
                .HasMaxLength(5);

            configuration.Property(c => c.CohortName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(c => c.LastPatientNo)
                .IsRequired()
                .HasDefaultValue(0);

            configuration.Property(c => c.StartDate)
                .IsRequired()
                .HasColumnType("date");

            configuration.Property(c => c.FinishDate)
                .HasColumnType("date");

            configuration.Property(c => c.MinEnrolment)
                .IsRequired(true)
                .HasDefaultValue(0);

            configuration.Property(c => c.MaxEnrolment)
                .IsRequired(true)
                .HasDefaultValue(0);

            configuration.Property(e => e.ConditionId)
                .IsRequired()
                .HasColumnName("Condition_Id");

            configuration.HasOne(d => d.Condition)
                .WithMany(p => p.CohortGroups)
                .HasForeignKey(d => d.ConditionId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => e.ConditionId);
            configuration.HasIndex("CohortName").IsUnique(true);
            configuration.HasIndex("CohortCode").IsUnique(true);
        }
    }
}
