using System.Security.Claims;

namespace Identity.Core;

public static class Extensions
{
    public static List<Claim> AddIfNotEmpty(this List<Claim> claims, string type, Guid value)
    {
        if (value != Guid.Empty)
            claims.Add(new Claim(type, value.ToString()));

        return claims;
    }
}