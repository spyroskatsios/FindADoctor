using System.Diagnostics.Metrics;

namespace Doctors.Api.Diagnostics;

public static class ApplicationDiagnostics
{
    private const string ServiceName = "Doctors.Api";
    public static readonly Meter Meter = new(ServiceName);
}