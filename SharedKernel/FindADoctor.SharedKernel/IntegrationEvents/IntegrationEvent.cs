using System.Diagnostics;
using System.Text.Json.Serialization;
using MediatR;

namespace FindADoctor.SharedKernel.IntegrationEvents;

[JsonDerivedType(typeof(OfficeCreatedIntegrationEvent), typeDiscriminator: nameof(OfficeCreatedIntegrationEvent))]
[JsonDerivedType(typeof(OfficeRemovedIntegrationEvent), typeDiscriminator: nameof(OfficeRemovedIntegrationEvent))]

public record IntegrationEvent : INotification
{
    public Guid Id { get; init; } = Guid.NewGuid();
}