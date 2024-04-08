using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NetCoreIdentityApp.Web.Models;
using System.Security.Claims;

namespace NetCoreIdentityApp.Web.ClaimProvider
{
    public class UserClaimProvider : IClaimsTransformation
    {
        private readonly UserManager<AppUser> _userManager;

        public UserClaimProvider(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identityUser = principal.Identity as ClaimsIdentity; // claim'lerden oluşan identity
            var currentUser = await _userManager.FindByNameAsync(identityUser.Name);

            if (String.IsNullOrEmpty(currentUser!.City ))
            {
                return principal;
            }

            if (currentUser.City != null)
            {

                if (principal.HasClaim(x=>x.Type != "city"))
                {
                    Claim cityClaim = new Claim("city", currentUser.City);
                    identityUser.AddClaim(cityClaim);
                }

            }

            return principal;

        }
    }
}
