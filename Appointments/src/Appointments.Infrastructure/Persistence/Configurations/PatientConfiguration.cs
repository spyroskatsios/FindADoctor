using Appointments.Domain.AppointmentAggregate;
using Appointments.Domain.PatientAggregate;
using Appointments.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Appointments.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, guid => PatientId.From(guid))
            .ValueGeneratedNever();

        builder.Property(x => x.UserId);
        
        builder.Property<List<AppointmentId>>("_appointmentIds")
            .HasColumnName("AppointmentIds")
            .HasConversion(x=> string.Join(',', x.Select(id => id.Value).ToList())
                , x => x.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(id=> AppointmentId.From(Guid.Parse(id))).ToList()
                , ValueComparers.ListComparer<AppointmentId>());
        
        builder.Ignore(x => x.AppointmentIds);
    }
}