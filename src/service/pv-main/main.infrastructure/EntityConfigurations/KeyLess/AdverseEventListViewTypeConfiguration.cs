using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities.Keyless;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations.KeyLess
{
    class AdverseEventListViewTypeConfiguration : IEntityTypeConfiguration<AdverseEventList>
    {
        public void Configure(EntityTypeBuilder<AdverseEventList> configuration)
        {
            configuration.ToView("vwAdverseEventList");

            configuration.HasNoKey();
        }
    }
}
