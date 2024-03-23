using Microsoft.AspNetCore.Identity;
using NetCoreIdentityApp.Web.CustomValidation;
using NetCoreIdentityApp.Web.Localizations;
using NetCoreIdentityApp.Web.Models;

namespace NetCoreIdentityApp.Web.Extensions
{
    public static class StartUpExtensions
    {
        // static anahtar kelimesi ise bu sınıfın nesne oluşturulmadan doğrudan kullanılabileceğini belirtir. Bu sınıfın tüm üyeleri de static olmalı
        public static void AddIdentityWithExt(this IServiceCollection services)
        {
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromSeconds(2); // oluşturacağım token'ın ömrünü yapılandırdırm
            });


            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                //options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz1234567890_";

                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;


                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1); // 1 dakika kilitlensin demek
                options.Lockout.MaxFailedAccessAttempts = 3;                      // 3 kez yanlış girme hakkı var 


            }).AddPasswordValidator<PasswordValidator>()
            .AddUserValidator<UserValidator>()
            .AddErrorDescriber<LocalizationIdentityErrorDescriber>()
            .AddDefaultTokenProviders() // kendisi token oluşturacak ve şifre sıfırlama için yapıyorum
            .AddEntityFrameworkStores<AppDbContext>();

        }
    }
}
