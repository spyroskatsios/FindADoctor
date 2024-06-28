using Appointments.Domain.AppointmentAggregate;
using Appointments.Domain.OfficeAggregate;
using Appointments.Domain.PatientAggregate;
using Microsoft.EntityFrameworkCore;

namespace Appointments.Application.Common.Interfaces;

public interface IReadDbContext
{
    DbSet<Office> Offices { get; }
    DbSet<Appointment> Appointments { get; } 
    DbSet<Patient> Patients { get; }
}