﻿using Microsoft.AspNetCore.Identity;
using NetCoreIdentityApp.Web.CustomValidation;
using NetCoreIdentityApp.Web.Models;

namespace NetCoreIdentityApp.Web.Extensions
{
    public static class StartUpExtensions
    {
        // static anahtar kelimesi ise bu sınıfın nesne oluşturulmadan doğrudan kullanılabileceğini belirtir. Bu sınıfın tüm üyeleri de static olmalı
        public static void AddIdentityWithExt(this IServiceCollection services)
        {
           services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyz1234567890_";

                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;


            }).AddPasswordValidator<PasswordValidator>().AddUserValidator<UserValidator>().AddEntityFrameworkStores<AppDbContext>();

        }
    }
}