using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class MetaTableEntityTypeConfiguration : IEntityTypeConfiguration<MetaTable>
    {
        public void Configure(EntityTypeBuilder<MetaTable> configuration)
        {
            configuration.ToTable("MetaTable");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.FriendlyDescription)
                .HasMaxLength(250);

            configuration.Property(e => e.FriendlyName)
                .HasMaxLength(100);

            configuration.Property(e => e.MetaTableGuid)
                .IsRequired()
                .HasColumnName("metatable_guid");

            configuration.Property(e => e.TableName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.TableTypeId)
                .IsRequired()
                .HasColumnName("TableType_Id");

            configuration.HasOne(d => d.TableType)
                .WithMany(p => p.MetaTables)
                .HasForeignKey(d => d.TableTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            configuration.HasIndex("TableName").IsUnique(true);
            configuration.HasIndex(e => e.TableTypeId);
        }
    }
}
