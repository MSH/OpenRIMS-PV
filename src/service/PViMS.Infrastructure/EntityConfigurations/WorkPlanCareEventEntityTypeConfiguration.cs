using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class WorkPlanCareEventEntityTypeConfiguration : IEntityTypeConfiguration<WorkPlanCareEvent>
    {
        public void Configure(EntityTypeBuilder<WorkPlanCareEvent> configuration)
        {
            configuration.ToTable("WorkPlanCareEvent");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.CareEventId)
                .IsRequired()
                .HasColumnName("CareEvent_Id");

            configuration.Property(e => e.WorkPlanId)
                .IsRequired()
                .HasColumnName("WorkPlan_Id");

            configuration.HasOne(d => d.CareEvent)
                .WithMany(p => p.WorkPlanCareEvents)
                .HasForeignKey(d => d.CareEventId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.WorkPlan)
                .WithMany(p => p.WorkPlanCareEvents)
                .HasForeignKey(d => d.WorkPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => new { e.WorkPlanId, e.CareEventId }).IsUnique(true);
            configuration.HasIndex(e => e.CareEventId);
            configuration.HasIndex(e => e.WorkPlanId);
        }
    }
}
