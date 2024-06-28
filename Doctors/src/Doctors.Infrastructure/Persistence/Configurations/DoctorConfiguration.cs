using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.OfficeAggregate;
using Doctors.Domain.SubscriptionAggregate;
using Doctors.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doctors.Infrastructure.Persistence.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => DoctorId.From(value));

        builder.Property(d => d.FirstName)
            .HasMaxLength(100); // TODO: Move to constants

        builder.Property(d => d.LastName)
            .HasMaxLength(100);

        builder.Property(d => d.Speciality)
            .HasConversion(speciality => speciality.Value,
                value => Speciality.FromValue(value));
        
        builder.Property(d => d.UserId);

        builder.Property(d => d.SubscriptionId)
            .HasConversion(id => id!.Value, value => SubscriptionId.From(value));

        builder.Property("_maxOffices")
            .HasColumnName("MaxOffices");

        builder.Property<List<OfficeId>>("_officeIds")
            .HasColumnName("OfficeIds")
            .HasConversion(o=> string.Join(',', o.Select(x => x.Value).ToList())
                , o => o.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x=> OfficeId.From(Guid.Parse(x))).ToList()
                , ValueComparers.ListComparer<OfficeId>());

        builder.Ignore(x => x.OfficeIds);
    }
}

