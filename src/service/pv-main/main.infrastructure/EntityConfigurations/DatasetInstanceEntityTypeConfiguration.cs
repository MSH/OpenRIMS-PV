using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;
using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class DatasetInstanceEntityTypeConfiguration : IEntityTypeConfiguration<DatasetInstance>
    {
        public void Configure(EntityTypeBuilder<DatasetInstance> configuration)
        {
            configuration.ToTable("DatasetInstance");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ContextId)
                .IsRequired()
                .HasColumnName("ContextID");

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.DatasetId)
                .IsRequired()
                .HasColumnName("Dataset_Id");

            configuration.Property(e => e.EncounterTypeWorkPlanId)
                .HasColumnName("EncounterTypeWorkPlan_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(c => c.DatasetInstanceGuid)
                .IsRequired()
                .HasDefaultValueSql("newid()");

            configuration.Property(c => c.Status)
                .HasConversion(x => (int)x, x => (DatasetInstanceStatus)x);

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.DatasetInstanceCreations)
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasOne(d => d.Dataset)
                .WithMany(p => p.DatasetInstances)
                .HasForeignKey(d => d.DatasetId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.EncounterTypeWorkPlan)
                .WithMany(p => p.DatasetInstances)
                .HasForeignKey(d => d.EncounterTypeWorkPlanId);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.DatasetInstanceUpdates)
                .HasForeignKey(d => d.UpdatedById)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.DatasetId);
            configuration.HasIndex(e => e.EncounterTypeWorkPlanId);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
