using Appointments.Application.Common.Authorization;

namespace Appointments.Application.Common.Models;

public record CurrentUser(string Id, IEnumerable<string> Permissions, IEnumerable<string> Roles);

public static class CurrentUserExtensions
{
    public static bool IsDoctor(this CurrentUser user) => user.Roles.Contains(AppRoles.Doctor);
    public static bool IsPatient(this CurrentUser user) => user.Roles.Contains(AppRoles.Patient);
}