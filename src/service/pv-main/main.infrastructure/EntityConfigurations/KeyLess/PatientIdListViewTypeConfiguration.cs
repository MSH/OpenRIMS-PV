using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities.Keyless;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations.KeyLess
{
    class PatientIdListViewTypeConfiguration : IEntityTypeConfiguration<PatientIdList>
    {
        public void Configure(EntityTypeBuilder<PatientIdList> configuration)
        {
            configuration.ToView("vwPatientIdList");

            configuration.HasNoKey();
        }
    }
}
