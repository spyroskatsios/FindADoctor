using Appointments.Application.Common.Models;
using TestCommon.TestConstants;

namespace TestCommon.Utils.Common;

public static class CurrentUserFactory
{
    public static CurrentUser CreateCurrentUser(string? userId = null, IEnumerable<string>? permissions = null, IEnumerable<string>? roles = null)
        => new(userId ?? Constants.User.Id, permissions ?? Enumerable.Empty<string>(), roles ?? Enumerable.Empty<string>());
}