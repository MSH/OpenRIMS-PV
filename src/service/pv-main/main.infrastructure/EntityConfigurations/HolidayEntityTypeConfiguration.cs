using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class HolidayEntityTypeConfiguration : IEntityTypeConfiguration<Holiday>
    {
        public void Configure(EntityTypeBuilder<Holiday> configuration)
        {
            configuration.ToTable("Holiday");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(100);

            configuration.Property(e => e.HolidayDate)
                .IsRequired();

            configuration.HasIndex("HolidayDate").IsUnique(true);
        }
    }
}
