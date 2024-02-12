using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities.Keyless;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations.KeyLess
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
