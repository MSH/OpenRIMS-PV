using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class ReportInstanceTaskEntityTypeConfiguration : IEntityTypeConfiguration<ReportInstanceTask>
    {
        public void Configure(EntityTypeBuilder<ReportInstanceTask> configuration)
        {
            configuration.ToTable("ReportInstanceTask");

            configuration.HasKey(e => e.Id);

            configuration
                .OwnsOne(o => o.TaskDetail, a =>
                {
                    a.WithOwner();
                });

            configuration
                .Property(c => c.TaskTypeId)
                .IsRequired();

            configuration
                .Property(c => c.TaskStatusId)
                .IsRequired();

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.HasOne(d => d.ReportInstance)
                .WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ReportInstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.ReportInstanceTaskCreations)
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.ReportInstanceTaskUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasIndex(e => e.ReportInstanceId);
        }
    }
}
