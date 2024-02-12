using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class ActivityExecutionStatusEntityTypeConfiguration : IEntityTypeConfiguration<ActivityExecutionStatus>
    {
        public void Configure(EntityTypeBuilder<ActivityExecutionStatus> configuration)
        {
            configuration.ToTable("ActivityExecutionStatus");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.ActivityId)
                .HasColumnName("Activity_Id");

            configuration.Property(c => c.FriendlyDescription)
                .HasMaxLength(100);

            configuration.HasOne(d => d.Activity)
                .WithMany(p => p.ExecutionStatuses)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => new { e.Description, e.ActivityId }).IsUnique(true);
            configuration.HasIndex(e => e.ActivityId);
        }
    }
}
