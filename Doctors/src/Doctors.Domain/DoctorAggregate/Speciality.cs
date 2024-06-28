using Ardalis.SmartEnum;
using Doctors.Domain.SubscriptionAggregate;

namespace Doctors.Domain.DoctorAggregate;

public sealed class Speciality : SmartEnum<Speciality>
{
    public static readonly Speciality Cardiologist = new(nameof(Cardiologist), 0);
    public static readonly Speciality Dermatologist = new(nameof(Dermatologist), 1);
    public static readonly Speciality Endocrinologist = new(nameof(Endocrinologist), 2);
    public static readonly Speciality Gastroenterologist = new(nameof(Gastroenterologist), 3);
    public static readonly Speciality Nephrologist = new(nameof(Nephrologist), 4);
    public static readonly Speciality Neurologist = new(nameof(Neurologist), 5);
    
    private Speciality(string name, int value) : base(name, value)
    {
    }
}
