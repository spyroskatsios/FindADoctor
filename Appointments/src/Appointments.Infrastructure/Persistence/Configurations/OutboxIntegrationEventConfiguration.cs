using Appointments.Infrastructure.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Appointments.Infrastructure.Persistence.Configurations;

public class OutboxIntegrationEventConfiguration : IEntityTypeConfiguration<OutboxIntegrationEvent>
{
    public void Configure(EntityTypeBuilder<OutboxIntegrationEvent> builder)
    {
        builder
            .Property<int>("Id")
            .ValueGeneratedOnAdd();

        builder.HasKey("Id");

        builder.Property(x => x.EventName);
        
        builder.Property(x => x.EventContent);
    }
}