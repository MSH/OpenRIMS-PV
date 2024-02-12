using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class ConfigEntityTypeConfiguration : IEntityTypeConfiguration<Config>
    {
        public void Configure(EntityTypeBuilder<Config> configuration)
        {
            configuration.ToTable("Config");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ConfigValue)
                .IsRequired(true)
                .HasMaxLength(100);

            configuration.Property(c => c.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(c => c.ConfigType)
                .HasConversion(x => (int)x, x => (ConfigType)x);

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.ConfigCreations)
                .HasForeignKey(d => d.CreatedById);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.ConfigUpdates)
                .HasForeignKey(d => d.UpdatedById);

            configuration.HasIndex("ConfigType").IsUnique(false);
            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
