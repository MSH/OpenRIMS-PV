using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class PriorityEntityTypeConfiguration : IEntityTypeConfiguration<Priority>
    {
        public void Configure(EntityTypeBuilder<Priority> configuration)
        {
            configuration.ToTable("Priority");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
