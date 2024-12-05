namespace Appointments.Contracts.Common;

public record HealthCheckResponse(string Status, IEnumerable<HealthCheck> HealthChecks, TimeSpan Duration);

public record HealthCheck(string Status, string Component, string Description);