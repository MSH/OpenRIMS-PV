using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities.Keyless;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations.KeyLess
{
    class DrugListViewTypeConfiguration : IEntityTypeConfiguration<DrugList>
    {
        public void Configure(EntityTypeBuilder<DrugList> configuration)
        {
            configuration.ToView("vwDrugList");

            configuration.HasNoKey();
        }
    }
}
