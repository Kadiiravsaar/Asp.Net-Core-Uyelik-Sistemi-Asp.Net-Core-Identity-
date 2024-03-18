using Microsoft.AspNetCore.Identity;
using NetCoreIdentityApp.Web.Models;

namespace NetCoreIdentityApp.Web.CustomValidation
{
    public class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var errors = new List<IdentityError>();
            var isDigit = int.TryParse(user.UserName![0].ToString(), out _); // int.TryParse kullanıcı adında ilk karekter sayısal mı ona bakar, 2. overload'ında ise eğer varsa bunu bir değişkene atat
                                                                             // fakat ben onu kullanmak istemiyorsam memory'de yer tutmasın istiyorusam _ kullanarak keserim

            if (isDigit)
            {
                errors.Add(new() { Code = "UserNameContainFirstLetterDigit", Description = "Kullanıcı adının ilk karekteri sayısal bir karakter içeremez" });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
