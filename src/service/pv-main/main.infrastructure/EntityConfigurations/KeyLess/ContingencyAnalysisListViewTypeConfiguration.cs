using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities.Keyless;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations.KeyLess
{
    class ContingencyAnalysisListViewTypeConfiguration : IEntityTypeConfiguration<ContingencyAnalysisList>
    {
        public void Configure(EntityTypeBuilder<ContingencyAnalysisList> configuration)
        {
            configuration.ToView("vwContingencyAnalysisList");

            configuration.HasNoKey();
        }
    }
}
