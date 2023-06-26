using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Aggregates.UserAggregate;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class RefreshTokenEntityTypeConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> configuration)
        {
            configuration.ToTable("RefreshToken");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Expires)
                .IsRequired();

            configuration.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("User_Id");

            configuration.HasOne(d => d.User)
                .WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex(e => e.UserId);
        }
    }
}
