using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities.Keyless;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations.KeyLess
{
    class ContingencyAnalysisItemViewTypeConfiguration : IEntityTypeConfiguration<ContingencyAnalysisItem>
    {
        public void Configure(EntityTypeBuilder<ContingencyAnalysisItem> configuration)
        {
            configuration.ToView("vwContingencyAnalysisItem");

            configuration.HasNoKey();

            configuration.Property(e => e.AdjustedRelativeRisk)
                .HasColumnType("Decimal")
                .HasPrecision(9, 2);

            configuration.Property(e => e.UnadjustedRelativeRisk)
                .HasColumnType("Decimal")
                .HasPrecision(9, 2);

            configuration.Property(e => e.ConfidenceIntervalHigh)
                .HasColumnType("Decimal")
                .HasPrecision(9, 2);

            configuration.Property(e => e.ConfidenceIntervalLow)
                .HasColumnType("Decimal")
                .HasPrecision(9, 2);

            configuration.Property(e => e.ExposedIncidenceRate)
                .HasColumnType("Decimal")
                .HasPrecision(9, 2);

            configuration.Property(e => e.ExposedPopulation)
                .HasColumnType("Decimal")
                .HasPrecision(9, 2);

            configuration.Property(e => e.NonExposedIncidenceRate)
                .HasColumnType("Decimal")
                .HasPrecision(9, 2);

            configuration.Property(e => e.NonExposedPopulation)
                .HasColumnType("Decimal")
                .HasPrecision(9, 2);
        }
    }
}
