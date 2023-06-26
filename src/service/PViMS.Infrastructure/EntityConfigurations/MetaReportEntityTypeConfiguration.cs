using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;
using PVIMS.Core.ValueTypes;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class MetaReportEntityTypeConfiguration : IEntityTypeConfiguration<MetaReport>
    {
        public void Configure(EntityTypeBuilder<MetaReport> configuration)
        {
            configuration.ToTable("MetaReport");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Breadcrumb)
                .HasMaxLength(250);

            configuration.Property(e => e.MetaReportGuid)
                .IsRequired()
                .HasColumnName("metareport_guid");

            configuration.Property(e => e.ReportDefinition)
                .HasMaxLength(250);

            configuration.Property(e => e.ReportName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.SqlDefinition)
                .HasColumnName("SQLDefinition");

            configuration.Property(c => c.IsSystem)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(c => c.ReportStatus)
                .HasConversion(x => (int)x, x => (MetaReportStatus)x);

            configuration.HasIndex("ReportName").IsUnique(true);
        }
    }
}
