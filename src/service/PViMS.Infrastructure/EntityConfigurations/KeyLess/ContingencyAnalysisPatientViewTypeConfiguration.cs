using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Keyless;

namespace PVIMS.Infrastructure.EntityConfigurations.KeyLess
{
    class ContingencyAnalysisPatientViewTypeConfiguration : IEntityTypeConfiguration<ContingencyAnalysisPatient>
    {
        public void Configure(EntityTypeBuilder<ContingencyAnalysisPatient> configuration)
        {
            configuration.ToView("vwContingencyAnalysisPatient");

            configuration.HasNoKey();
        }
    }
}
