using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class EncounterTypeWorkPlanEntityTypeConfiguration : IEntityTypeConfiguration<EncounterTypeWorkPlan>
    {
        public void Configure(EntityTypeBuilder<EncounterTypeWorkPlan> configuration)
        {
            configuration.ToTable("EncounterTypeWorkPlan");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.CohortGroupId)
                .HasColumnName("CohortGroup_Id");

            configuration.Property(e => e.EncounterTypeId)
                .IsRequired()
                .HasColumnName("EncounterType_Id");

            configuration.Property(e => e.WorkPlanId)
                .IsRequired()
                .HasColumnName("WorkPlan_Id");

            configuration.HasOne(d => d.CohortGroup)
                .WithMany(p => p.EncounterTypeWorkPlans)
                .HasForeignKey(d => d.CohortGroupId);

            configuration.HasOne(d => d.EncounterType)
                .WithMany(p => p.EncounterTypeWorkPlans)
                .HasForeignKey(d => d.EncounterTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.WorkPlan)
                .WithMany(p => p.EncounterTypeWorkPlans)
                .HasForeignKey(d => d.WorkPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => new { e.CohortGroupId, e.EncounterTypeId, e.WorkPlanId }).IsUnique(true);
            configuration.HasIndex(e => e.CohortGroupId);
            configuration.HasIndex(e => e.EncounterTypeId);
            configuration.HasIndex(e => e.WorkPlanId);
        }
    }
}
