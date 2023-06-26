using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Keyless;

namespace PVIMS.Infrastructure.EntityConfigurations.KeyLess
{
    class MetaPatientListViewTypeConfiguration : IEntityTypeConfiguration<MetaPatientList>
    {
        public void Configure(EntityTypeBuilder<MetaPatientList> configuration)
        {
            configuration.ToView("vwMetaPatientList");

            configuration.HasNoKey();
        }
    }
}
