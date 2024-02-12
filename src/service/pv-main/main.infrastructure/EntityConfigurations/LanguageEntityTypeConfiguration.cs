using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class LanguageEntityTypeConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> configuration)
        {
            configuration.ToTable("Language");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(20);

            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
