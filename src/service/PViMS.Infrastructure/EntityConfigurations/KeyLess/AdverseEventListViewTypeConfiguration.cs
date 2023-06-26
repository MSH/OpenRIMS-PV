using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities.Keyless;

namespace PVIMS.Infrastructure.EntityConfigurations.KeyLess
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
