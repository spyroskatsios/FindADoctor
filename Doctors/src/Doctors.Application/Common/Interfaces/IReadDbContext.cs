using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.OfficeAggregate;
using Doctors.Domain.SubscriptionAggregate;
using Microsoft.EntityFrameworkCore;

namespace Doctors.Application.Common.Interfaces;

public interface IReadDbContext
{
    DbSet<Doctor> Doctors { get; }
    DbSet<Office> Offices { get; }
    DbSet<Subscription> Subscriptions { get; } 
}