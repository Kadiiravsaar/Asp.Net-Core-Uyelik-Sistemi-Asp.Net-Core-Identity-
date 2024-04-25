using Microsoft.AspNetCore.Identity;
using NetCoreIdentityApp.Web.Models;
using System.Security.Claims;

namespace NetCoreIdentityApp.Web.Seeds
{
    public class PermissionSeed
    {
        public static async void Seed(RoleManager<AppRole> roleManager)
        {
            var hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");

            if (!hasBasicRole)
            {
                await roleManager.CreateAsync(new AppRole { Name = "BasicRole" });

                var basicRole = (await roleManager.FindByNameAsync("BasicRole"))!;

                await roleManager.AddClaimAsync(basicRole, new Claim("Permisson", PermissionsRoot.Permission.Stock.Read));

                await roleManager.AddClaimAsync(basicRole, new Claim("Permisson", PermissionsRoot.Permission.Order.Read));

                await roleManager.AddClaimAsync(basicRole, new Claim("Permisson", PermissionsRoot.Permission.Catalog.Read));

            }
        }
    }
}
