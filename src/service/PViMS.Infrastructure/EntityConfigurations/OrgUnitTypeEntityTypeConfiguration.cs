using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class OrgUnitTypeEntityTypeConfiguration : IEntityTypeConfiguration<OrgUnitType>
    {
        public void Configure(EntityTypeBuilder<OrgUnitType> configuration)
        {
            configuration.ToTable("OrgUnitType");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.ParentId)
                .HasColumnName("Parent_Id");

            configuration.HasOne(d => d.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(d => d.ParentId);

            configuration.HasIndex("Description").IsUnique(true);
            configuration.HasIndex(e => e.ParentId);
        }
    }
}
