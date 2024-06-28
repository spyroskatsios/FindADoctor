using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.SubscriptionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doctors.Infrastructure.Persistence.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.Property(s => s.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => SubscriptionId.From(value));

        builder.Property(s => s.DoctorId)
            .HasConversion(id => id.Value, value => DoctorId.From(value));
        
        builder.Property(s => s.SubscriptionType)
            .HasConversion(
                subscriptionType => subscriptionType.Value,
                value => SubscriptionType.FromValue(value));
    }
}