using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PVIMS.Core.Entities;

namespace PVIMS.Infrastructure.EntityConfigurations
{
    class SystemLogEntityTypeConfiguration : IEntityTypeConfiguration<SystemLog>
    {
        public void Configure(EntityTypeBuilder<SystemLog> configuration)
        {
            configuration.ToTable("SystemLog");

            configuration.HasKey(e => e.Id);

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.EventType)
                .IsRequired();

            configuration.Property(e => e.ExceptionCode)
                .IsRequired();

            configuration.Property(e => e.ExceptionMessage)
                .IsRequired();

            configuration.Property(e => e.Sender)
                .IsRequired();

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.SystemLogCreations)
                .HasForeignKey(d => d.CreatedById);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.SystemLogUpdates)
                .HasForeignKey(d => d.UpdatedById);

            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.UpdatedById);
        }
    }
}
