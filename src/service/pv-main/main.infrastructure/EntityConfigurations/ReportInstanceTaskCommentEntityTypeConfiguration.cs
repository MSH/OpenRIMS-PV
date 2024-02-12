using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Aggregates.ReportInstanceAggregate;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class ReportInstanceTaskCommentEntityTypeConfiguration : IEntityTypeConfiguration<ReportInstanceTaskComment>
    {
        public void Configure(EntityTypeBuilder<ReportInstanceTaskComment> configuration)
        {
            configuration.ToTable("ReportInstanceTaskComment");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Comment)
                .HasMaxLength(500)
                .IsRequired();

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.HasOne(d => d.ReportInstanceTask)
                .WithMany(p => p.Comments)
                .HasForeignKey(d => d.ReportInstanceTaskId)
                .OnDelete(DeleteBehavior.Cascade);
            
            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.ReportInstanceTaskCommentCreations)
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.ReportInstanceTaskCommentUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasIndex(e => e.ReportInstanceTaskId);
        }
    }
}
