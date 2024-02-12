using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.ValueTypes;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class AuditLogEntityTypeConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> configuration)
        {
            configuration.ToTable("AuditLog");

            configuration.HasKey(e => e.Id);

            configuration.Property(c => c.ActionDate)
                .IsRequired();

            configuration.Property(c => c.AuditType)
                .HasConversion(x => (int)x, x => (AuditType)x);

            configuration.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("User_Id");

            configuration.HasOne(d => d.User)
                .WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            configuration.HasIndex("ActionDate").IsUnique(false);
            configuration.HasIndex(e => e.UserId);
        }
    }
}
