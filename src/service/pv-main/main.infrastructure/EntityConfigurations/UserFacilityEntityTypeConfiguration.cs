using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class UserFacilityEntityTypeConfiguration : IEntityTypeConfiguration<UserFacility>
    {
        public void Configure(EntityTypeBuilder<UserFacility> configuration)
        {
            configuration.ToTable("UserFacility");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.FacilityId)
                .IsRequired()
                .HasColumnName("Facility_Id");

            configuration.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("User_Id");

            configuration.HasOne(d => d.Facility)
                .WithMany(p => p.UserFacilities)
                .HasForeignKey(d => d.FacilityId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.User)
                .WithMany(p => p.Facilities)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => new { e.UserId, e.FacilityId }).IsUnique(true);
            configuration.HasIndex(e => e.FacilityId);
            configuration.HasIndex(e => e.UserId);
        }
    }
}
