using Doctors.Infrastructure.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doctors.Infrastructure.Persistence.Configurations;

public class OutboxIntegrationEventConfiguration : IEntityTypeConfiguration<OutboxIntegrationEvent>
{
    public void Configure(EntityTypeBuilder<OutboxIntegrationEvent> builder)
    {
        builder.Property<int>("Id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.EventName);
        
        builder.Property(x => x.EventContent);

        builder.Property(x => x.ActivityExtractedContext);
    }
}