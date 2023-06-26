using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class PostDeploymentEntityTypeConfiguration : IEntityTypeConfiguration<PostDeployment>
    {
        public void Configure(EntityTypeBuilder<PostDeployment> configuration)
        {
            configuration.ToTable("PostDeployment");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.RunDate)
                .HasPrecision(0);

            configuration.Property(e => e.ScriptDescription)
                .IsRequired()
                .HasMaxLength(200);

            configuration.Property(e => e.ScriptFileName)
                .IsRequired()
                .HasMaxLength(200);
                
            configuration.HasIndex(e => e.ScriptFileName).IsUnique();
        }
    }
}
