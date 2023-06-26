using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class WorkFlowEntityTypeConfiguration : IEntityTypeConfiguration<WorkFlow>
    {
        public void Configure(EntityTypeBuilder<WorkFlow> configuration)
        {
            configuration.ToTable("WorkFlow");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(100);

            configuration.Property(e => e.WorkFlowGuid)
                .IsRequired();

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
