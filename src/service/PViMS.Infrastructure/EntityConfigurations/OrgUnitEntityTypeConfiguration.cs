using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class OrgUnitEntityTypeConfiguration : IEntityTypeConfiguration<OrgUnit>
    {
        public void Configure(EntityTypeBuilder<OrgUnit> configuration)
        {
            configuration.ToTable("OrgUnit");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.OrgUnitTypeId)
                .IsRequired()
                .HasColumnName("OrgUnitType_Id");

            configuration.Property(e => e.ParentId)
                .HasColumnName("Parent_Id");

            configuration.HasOne(d => d.OrgUnitType)
                .WithMany(p => p.OrgUnits)
                .HasForeignKey(d => d.OrgUnitTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasOne(d => d.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasIndex("Name").IsUnique(true);
            configuration.HasIndex(e => e.OrgUnitTypeId);
            configuration.HasIndex(e => e.ParentId);
        }
    }
}
