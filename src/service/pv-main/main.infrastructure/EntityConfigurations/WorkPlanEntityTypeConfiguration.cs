using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class WorkPlanEntityTypeConfiguration : IEntityTypeConfiguration<WorkPlan>
    {
        public void Configure(EntityTypeBuilder<WorkPlan> configuration)
        {
            configuration.ToTable("WorkPlan");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.DatasetId).HasColumnName("Dataset_Id");

            configuration.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.HasOne(d => d.Dataset)
                .WithMany(p => p.WorkPlans)
                .HasForeignKey(d => d.DatasetId);

            configuration.HasIndex("Description").IsUnique(true);
            configuration.HasIndex(e => e.DatasetId);
        }
    }
}
