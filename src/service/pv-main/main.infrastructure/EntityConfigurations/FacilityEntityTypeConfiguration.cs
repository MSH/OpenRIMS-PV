using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class FacilityEntityTypeConfiguration : IEntityTypeConfiguration<Facility>
    {
        public void Configure(EntityTypeBuilder<Facility> configuration)
        {
            configuration.ToTable("Facility");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.FacilityCode)
                .IsRequired()
                .HasMaxLength(18);

            configuration.Property(c => c.FacilityName)
                .IsRequired()
                .HasMaxLength(100);

            configuration.Property(e => e.FacilityTypeId)
                .IsRequired()
                .HasColumnName("FacilityType_Id");

            configuration.Property(c => c.FaxNumber)
                .HasMaxLength(30);

            configuration.Property(c => c.MobileNumber)
                .HasMaxLength(30);

            configuration.Property(e => e.OrgUnitId)
                .HasColumnName("OrgUnit_Id");

            configuration.Property(c => c.TelNumber)
                .HasMaxLength(30);

            configuration.HasOne(d => d.FacilityType)
                .WithMany(p => p.Facilities)
                .HasForeignKey(d => d.FacilityTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.OrgUnit)
                .WithMany(p => p.Facilities)
                .HasForeignKey(d => d.OrgUnitId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex("FacilityCode").IsUnique(true);
            configuration.HasIndex("FacilityName").IsUnique(true);
            configuration.HasIndex(e => e.FacilityTypeId);
            configuration.HasIndex(e => e.OrgUnitId);
        }
    }
}
