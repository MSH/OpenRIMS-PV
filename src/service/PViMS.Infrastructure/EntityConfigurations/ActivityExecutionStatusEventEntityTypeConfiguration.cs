using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ActivityExecutionStatusEventEntityTypeConfiguration : IEntityTypeConfiguration<ActivityExecutionStatusEvent>
    {
        public void Configure(EntityTypeBuilder<ActivityExecutionStatusEvent> configuration)
        {
            configuration.ToTable("ActivityExecutionStatusEvent");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ActivityInstanceId)
                .IsRequired()
                .HasColumnName("ActivityInstance_Id");

            configuration.Property(c => c.ContextCode)
                .HasMaxLength(20);

            configuration.Property(e => e.EventCreatedById)
                .IsRequired()
                .HasColumnName("EventCreatedBy_Id");

            configuration.Property(e => e.ExecutionStatusId)
                .IsRequired()
                .HasColumnName("ExecutionStatus_Id");

            configuration.HasOne(d => d.ActivityInstance)
                .WithMany(p => p.ExecutionEvents)
                .HasForeignKey(d => d.ActivityInstanceId)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.EventCreatedBy)
                .WithMany(p => p.ExecutionEvents)
                .HasForeignKey(d => d.EventCreatedById);

            configuration.HasOne(d => d.ExecutionStatus)
                .WithMany(p => p.ExecutionEvents)
                .HasForeignKey(d => d.ExecutionStatusId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => new { e.EventDateTime, e.ActivityInstanceId, e.ExecutionStatusId }).IsUnique(true);
            configuration.HasIndex(e => e.ActivityInstanceId);
            configuration.HasIndex(e => e.EventCreatedById);
            configuration.HasIndex(e => e.ExecutionStatusId);
        }
    }
}
