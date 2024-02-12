using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class DatasetInstanceSubValueEntityTypeConfiguration : IEntityTypeConfiguration<DatasetInstanceSubValue>
    {
        public void Configure(EntityTypeBuilder<DatasetInstanceSubValue> configuration)
        {
            configuration.ToTable("DatasetInstanceSubValue");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.DatasetElementSubId)
                .IsRequired()
                .HasColumnName("DatasetElementSub_Id");

            configuration.Property(e => e.DatasetInstanceValueId)
                .IsRequired()
                .HasColumnName("DatasetInstanceValue_Id");

            configuration.Property(c => c.ContextValue)
                .IsRequired();

            configuration.Property(c => c.InstanceValue)
                .IsRequired();

            configuration.HasOne(d => d.DatasetElementSub)
                .WithMany(p => p.DatasetInstanceSubValues)
                .HasForeignKey(d => d.DatasetElementSubId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasOne(d => d.DatasetInstanceValue)
                .WithMany(p => p.DatasetInstanceSubValues)
                .HasForeignKey(d => d.DatasetInstanceValueId)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasIndex(e => new { e.ContextValue, e.DatasetInstanceValueId, e.DatasetElementSubId }).IsUnique(true);
            configuration.HasIndex(e => e.DatasetElementSubId);
            configuration.HasIndex(e => e.DatasetInstanceValueId);
        }
    }
}