using Appointments.Domain.AppointmentAggregate;
using Appointments.Domain.Common.ValueObjects;
using Appointments.Domain.OfficeAggregate;
using Appointments.Domain.PatientAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Appointments.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, guid => AppointmentId.From(guid))
            .ValueGeneratedNever();

        builder.Property(x => x.OfficeId)
            .HasConversion(id => id.Value, guid => OfficeId.From(guid));

        builder.Property(x => x.DoctorId)
            .HasConversion(id => id.Value, guid => DoctorId.From(guid));

        builder.Property(x => x.PatientId)
            .HasConversion(id => id.Value, guid => PatientId.From(guid));
        
        builder.Property(x => x.Date);

        builder.OwnsOne(x => x.TimeRange);

        builder.Property(x => x.Status)
            .HasConversion(
                status => status.Value,
                value => AppointmentStatus.FromValue(value));
    }
}