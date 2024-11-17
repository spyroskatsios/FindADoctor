using System.Diagnostics.Metrics;

namespace Appointments.Api.Diagnostics;

public static class ApplicationDiagnostics
{
    private const string ServiceName = "Doctors.Api";
    public static readonly Meter Meter = new(ServiceName);

    public static readonly Counter<long> AppointmentsBooked = Meter.CreateCounter<long>("appointments"); 
}