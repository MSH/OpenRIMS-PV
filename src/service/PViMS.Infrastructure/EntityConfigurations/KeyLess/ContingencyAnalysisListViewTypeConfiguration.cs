using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Keyless;

namespace PVIMS.Infrastructure.EntityConfigurations.KeyLess
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
