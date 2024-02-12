using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class SelectionDataItemEntityTypeConfiguration : IEntityTypeConfiguration<SelectionDataItem>
    {
        public void Configure(EntityTypeBuilder<SelectionDataItem> configuration)
        {
            configuration.ToTable("SelectionDataItem");

            configuration.HasKey(e => e.Id);

            configuration.HasIndex(new string[] { "AttributeKey", "SelectionKey" }).IsUnique(true);
        }
    }
}
