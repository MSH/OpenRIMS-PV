using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class AttachmentTypeEntityTypeConfiguration : IEntityTypeConfiguration<AttachmentType>
    {
        public void Configure(EntityTypeBuilder<AttachmentType> configuration)
        {
            configuration.ToTable("AttachmentType");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(50);

            configuration.Property(c => c.Key)
                .IsRequired()
                .HasMaxLength(4);

            configuration.HasIndex("Key").IsUnique(true);
        }
    }
}
