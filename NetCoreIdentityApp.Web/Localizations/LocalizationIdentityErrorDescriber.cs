using Microsoft.AspNetCore.Identity;

namespace NetCoreIdentityApp.Web.Localizations
{
    public class LocalizationIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {

            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"Kullanıcı adı '{userName}' daha önce başka bir kullanıcı tarafından alınmıştır."
            };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = $"E-posta adresi '{email}' daha önce başka bir kullanıcı tarafından alınmıştır."
            };
        }
    }
}
