﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCoreIdentityApp.Web.Extensions;
using NetCoreIdentityApp.Web.Models;
using NetCoreIdentityApp.Web.Services;
using NetCoreIdentityApp.Web.ViewModels;
using System.Diagnostics;
using System.Drawing;
using System.Security.Claims;
using System.Web;

namespace NetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;


        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager = null, SignInManager<AppUser> signInManager = null, IEmailService emailService = null)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }



        public IActionResult SignUp()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var identityResult = await _userManager.CreateAsync(new()// kendi içerisinde bulunan bi iş kuralı var ve username göre var yok kontrolu yapıyor
            {
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email
            }, request.PasswordConfirm);


            if (!identityResult.Succeeded)
            {
                ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
                return View();
            }

            var exchangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());

            var user = await _userManager.FindByNameAsync(request.UserName);

            var claimResult = await _userManager.AddClaimAsync(user!, exchangeExpireClaim);


            if (!claimResult.Succeeded)
            {
                ModelState.AddModelErrorList(claimResult.Errors.Select(x => x.Description).ToList());
                return View();
            }


            TempData["SuccessMessage"] = "Üyelik kayıt işlemi başarıla gerçekleşmiştir.";

            return RedirectToAction(nameof(HomeController.SignUp));
        }



        public IActionResult SignIn()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel request, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            returnUrl ??= Url.Action("Index", "Home");

            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            if (hasUser == null)
            {

                ModelState.AddModelError(string.Empty, "E-posta ya da şifre hatalı.");
                return View();
            }

            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, request.Password, isPersistent: request.RememberMe, lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "1 dakika boyunca giriş yapamazsın");
                return View();
            }

            if (hasUser.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(hasUser, request.RememberMe, new[] { new Claim("birthdate", hasUser.BirthDate.Value.ToString()) });
            }


            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl!);
            }

          

            ModelState.AddModelErrorList(new List<string>()
            {
                "E-posta ya da şifre hatalı."
            });

            return View();

        }


        public IActionResult ForgotPassword() // Şifremi unuttum
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel request) // Şifremi unuttum
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                ModelState.AddModelError(String.Empty, "Bu E-Postaya Sahip kullanıcı bulunamadı");
                return View();
            }

            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // bir link üretmek lazım

            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = user.Id, Token = passwordResetToken }, HttpContext.Request.Scheme);

            //  https://localhost:7006? userId =12345& token =a sdad123asd3

            await _emailService.SendResetPasswordEmail(passwordResetLink, user.Email);

            TempData["SuccesMessage"] = "Şifre yenileme linki e-posta adresinize gönderildi";
            return RedirectToAction(nameof(ForgotPassword));
        }



        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();

        } // maile gelen linke tıklanınca

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
        {
            var userId = TempData["userId"];
            var token = TempData["token"];

            if (userId == null || token == null)
            {
                throw new Exception("Bir hata meydana geldi");
            }

            var hasUser = await _userManager.FindByIdAsync(userId!.ToString());

            if (hasUser == null)
            {

                ModelState.AddModelError(String.Empty, "Kullanıcı bulunamamıştır.");
                return View();
            }


            IdentityResult result = await _userManager.ResetPasswordAsync(hasUser, token!.ToString(), request.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla yenilenmiştir";
            }

            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());


            }

            return View();

        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}
