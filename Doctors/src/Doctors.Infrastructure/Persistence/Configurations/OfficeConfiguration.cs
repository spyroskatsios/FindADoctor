using Doctors.Domain.Common.ValueObjects;
using Doctors.Domain.DoctorAggregate;
using Doctors.Domain.OfficeAggregate;
using Doctors.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doctors.Infrastructure.Persistence.Configurations;

public class OfficeConfiguration : IEntityTypeConfiguration<Office>
{
    public void Configure(EntityTypeBuilder<Office> builder)
    {
        builder.HasKey(o => o.Id);
        
        builder.Property(d => d.Id)
            .ValueGeneratedNever()
            .HasConversion(id => id.Value, value => OfficeId.From(value));

        builder.Property(x => x.DoctorId)
            .HasConversion(id => id.Value, value => DoctorId.From(value));

        builder.Property(x => x.Deleted);
        
        builder.HasQueryFilter(x=>x.Deleted == false);

        builder.OwnsOne(x => x.Address)
            .ToJson();
    }
}
