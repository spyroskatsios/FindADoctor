using Appointments.Domain.AppointmentAggregate;
using Appointments.Domain.Common.ValueObjects;
using Appointments.Domain.OfficeAggregate;
using Appointments.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Appointments.Infrastructure.Persistence.Configurations;

public class OfficeConfiguration : IEntityTypeConfiguration<Office>
{
    public void Configure(EntityTypeBuilder<Office> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, guid => OfficeId.From(guid))
            .ValueGeneratedNever();

        builder.Property(x => x.DoctorId)
            .HasConversion(id => id.Value, guid => DoctorId.From(guid));

        builder.Property(x => x.Deleted);

        builder.HasQueryFilter(x => x.Deleted == false);
        
        builder.Property<List<AppointmentId>>("_appointmentIds")
            .HasColumnName("AppointmentIds")
            .HasConversion(x=> string.Join(',', x.Select(id => id.Value).ToList())
                , x => x.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(id=> AppointmentId.From(Guid.Parse(id))).ToList()
                , ValueComparers.ListComparer<AppointmentId>());
        
        builder.Ignore(x => x.AppointmentIds);

        
        builder.OwnsOne(x => x.WorkingSchedule, navigationBuilder =>
        {
            navigationBuilder.Property(x=>x.Calendar) 
                .HasColumnName("WorkingScheduleCalendar")
                .HasConversion(ValueConverters.JsonConverter<Dictionary<DateOnly, List<TimeRange>>>(),
                    ValueComparers.JsonComparer<Dictionary<DateOnly, List<TimeRange>>>());
                
            navigationBuilder.Property(x => x.Id)
                .HasConversion(id => id.Value, guid => WorkingScheduleId.From(guid))
                .HasColumnName("WorkingScheduleId");
        } );
        
        builder.OwnsOne(x => x.BookedSchedule, navigationBuilder =>
        {
            navigationBuilder.Property(x=>x.Calendar)
                .HasColumnName("BookedScheduleCalendar")
                .HasConversion(ValueConverters.JsonConverter<Dictionary<DateOnly, List<TimeRange>>>(),
                    ValueComparers.JsonComparer<Dictionary<DateOnly, List<TimeRange>>>());
                
            navigationBuilder.Property(x => x.Id)
                .HasConversion(id => id.Value, guid => BookedScheduleId.From(guid))
                .HasColumnName("BookedScheduleId");
        } );
    }
}