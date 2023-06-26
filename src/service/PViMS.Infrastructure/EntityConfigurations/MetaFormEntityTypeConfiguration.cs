using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class MetaFormEntityTypeConfiguration : IEntityTypeConfiguration<MetaForm>
    {
        public void Configure(EntityTypeBuilder<MetaForm> configuration)
        {
            configuration.ToTable("MetaForm");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.ActionName)
                .IsRequired()
                .HasMaxLength(20);

            configuration.Property(e => e.FormName)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(e => e.IsSystem)
                .IsRequired()
                .HasDefaultValue(false);

            configuration.Property(e => e.MetaFormGuid)
                .IsRequired()
                .HasColumnName("metaform_guid");

            configuration.HasIndex("FormName").IsUnique(true);
        }
    }
}
