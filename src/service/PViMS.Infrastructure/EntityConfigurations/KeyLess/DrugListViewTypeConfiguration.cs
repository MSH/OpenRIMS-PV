using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Keyless;

namespace PVIMS.Infrastructure.EntityConfigurations.KeyLess
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
