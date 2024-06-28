using Doctors.Infrastructure.IntegrationEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doctors.Infrastructure.Persistence.Configurations;

public class ConsumedIntegrationEventConfiguration : IEntityTypeConfiguration<ConsumedIntegrationEvent>
{
    public void Configure(EntityTypeBuilder<ConsumedIntegrationEvent> builder)
    {
        builder.HasKey(x=> new {x.Id, x.Handler});
    }
}