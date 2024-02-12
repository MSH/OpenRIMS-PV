using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class TerminologyIcd10EntityTypeConfiguration : IEntityTypeConfiguration<TerminologyIcd10>
    {
        public void Configure(EntityTypeBuilder<TerminologyIcd10> configuration)
        {
            configuration.ToTable("TerminologyIcd10");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Name)
                .HasMaxLength(20);

            configuration.HasIndex("Name").IsUnique(true);
            configuration.HasIndex("Description").IsUnique(true);
        }
    }
}
