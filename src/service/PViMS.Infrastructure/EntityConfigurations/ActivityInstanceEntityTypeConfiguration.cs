using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Aggregates.ReportInstanceAggregate;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class ActivityInstanceEntityTypeConfiguration : IEntityTypeConfiguration<ActivityInstance>
    {
        public void Configure(EntityTypeBuilder<ActivityInstance> configuration)
        {
            configuration.ToTable("ActivityInstance");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.CurrentStatusId)
                .IsRequired()
                .HasColumnName("CurrentStatus_Id");

            configuration.Property(c => c.QualifiedName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.ReportInstanceId)
                .IsRequired()
                .HasColumnName("ReportInstance_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(c => c.Current)
                .IsRequired();

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.ActivityInstanceCreations)
                .HasForeignKey(d => d.CreatedById);

            configuration.HasOne(d => d.CurrentStatus)
                .WithMany(p => p.ActivityInstances)
                .HasForeignKey(d => d.CurrentStatusId);

            configuration.HasOne(d => d.ReportInstance)
                .WithMany(p => p.Activities)
                .HasForeignKey(d => d.ReportInstanceId)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.ActivityInstanceUpdates)
                .HasForeignKey(d => d.UpdatedById);

            configuration.HasIndex(e => new { e.QualifiedName, e.ReportInstanceId }).IsUnique(true);
            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.CurrentStatusId);
            configuration.HasIndex(e => e.ReportInstanceId);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
