using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
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
