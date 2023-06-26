using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Keyless;

namespace PVIMS.Infrastructure.EntityConfigurations.KeyLess
{
    class PatientOnStudyListViewTypeConfiguration : IEntityTypeConfiguration<PatientOnStudyList>
    {
        public void Configure(EntityTypeBuilder<PatientOnStudyList> configuration)
        {
            configuration.ToView("vwPatientOnStudyVisitList");

            configuration.HasNoKey();
        }
    }
}
