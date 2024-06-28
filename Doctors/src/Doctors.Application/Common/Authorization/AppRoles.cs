using System.Reflection;

namespace Doctors.Application.Common.Authorization;

public static class AppRoles
{
    public const string Doctor = "Doctor"; 
    public const string Patient = "Patient";
    
    public static IEnumerable<string> GetAll()
    {
        var roles = typeof(AppRoles).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(x => x.IsLiteral && x.FieldType == typeof(string))
            .Select(x=>x.GetRawConstantValue() as string).ToList();

        return roles!;
    }
}