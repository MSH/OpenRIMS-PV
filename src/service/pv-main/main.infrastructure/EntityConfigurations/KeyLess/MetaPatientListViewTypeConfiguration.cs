using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities.Keyless;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations.KeyLess
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
