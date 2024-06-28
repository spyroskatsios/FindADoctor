using Identity.Core.App.Common.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Identity.Core.Services;

public class SeedData
{
    public async Task CreateRoles(RoleManager<IdentityRole> manager)
    {
        var roles = AppRoles.GetAll();
        
        foreach (var role in roles)
        {
            if (!await manager.RoleExistsAsync(role))
                await manager.CreateAsync(new IdentityRole(role));
        }
    }
}