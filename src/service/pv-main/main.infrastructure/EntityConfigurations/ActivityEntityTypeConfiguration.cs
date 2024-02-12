using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class ActivityEntityTypeConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> configuration)
        {
            configuration.ToTable("Activity");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.QualifiedName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(c => c.ActivityType)
                .HasConversion(x => (int)x, x => (ActivityTypes)x);

            configuration.Property(e => e.WorkFlowId)
                .IsRequired()
                .HasColumnName("WorkFlow_Id");

            configuration.HasOne(c => c.WorkFlow)
                .WithMany(p => p.Activities)
                .HasForeignKey(c => c.WorkFlowId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => new { e.QualifiedName, e.WorkFlowId }).IsUnique(true);
            configuration.HasIndex(e => e.WorkFlowId);
        }
    }
}
