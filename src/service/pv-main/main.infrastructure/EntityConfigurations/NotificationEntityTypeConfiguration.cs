using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Aggregates.NotificationAggregate;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations
{
    class NotificationEntityTypeConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> configuration)
        {
            configuration.ToTable("Notification");

            configuration.HasKey(e => e.Id);

            configuration.Ignore(b => b.DomainEvents);

            configuration.Property(e => e.Created)
                .IsRequired();

            configuration.Property(e => e.CreatedById)
                .IsRequired()
                .HasColumnName("CreatedBy_Id");

            configuration.Property(e => e.UpdatedById)
                .HasColumnName("UpdatedBy_Id");

            configuration.Property(e => e.DestinationUserId)
                .IsRequired()
                .HasColumnName("DestinationUser_Id");

            configuration.Property(b => b.Summary)
                .HasMaxLength(100)
                .IsRequired();

            configuration
                .Property(c => c.NotificationTypeId)
                .IsRequired();

            configuration
                .Property(c => c.NotificationClassificationId)
                .IsRequired();

            configuration.Property(b => b.ContextRoute)
                .HasMaxLength(100)
                .IsRequired();

            configuration.HasOne(d => d.CreatedBy)
                .WithMany(p => p.NotificationCreations)
                .HasForeignKey(d => d.CreatedById);

            configuration.HasOne(d => d.UpdatedBy)
                .WithMany(p => p.NotificationUpdates)
                .HasForeignKey(d => d.UpdatedById);

            configuration.HasOne(d => d.DestinationUser)
                .WithMany(p => p.Notifications)
                .HasForeignKey(d => d.DestinationUserId)
                .OnDelete(DeleteBehavior.NoAction);

            configuration.HasIndex(e => e.CreatedById);
            configuration.HasIndex(e => e.UpdatedById);
            configuration.HasIndex(e => e.DestinationUserId);
        }
    }
}
