using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Keyless;

namespace PVIMS.Infrastructure.EntityConfigurations.KeyLess
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
