using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCoreIdentityApp.Web.Extensions;
using NetCoreIdentityApp.Web.Models;
using NetCoreIdentityApp.Web.ViewModels;
using System.Diagnostics;

namespace NetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager = null, SignInManager<AppUser> signInManager = null)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
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

        public IActionResult SignIn()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel request, string? returnUrl = null)
        {
            
            returnUrl = returnUrl ?? Url.Action("Index", "Home");
            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            if (hasUser == null)
            {

                ModelState.AddModelError(string.Empty, "E-posta ya da şifre hatalı.");
                return View();
            }

            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, request.Password, isPersistent: request.RememberMe, lockoutOnFailure: true);

            if (signInResult.Succeeded)
            {
                return RedirectToAction("Index","Member");
            }

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "1 dakika boyunca giri yapamazsın");
                return View();
            }


            ModelState.AddModelErrorList(new List<string>()
            {
                "E-posta ya da şifre hatalı."
            });

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

            if (identityResult.Succeeded)
            {
                TempData["SuccesMessage"] = "Üye başarılı bir şekilde kayıt edildi....";
                return RedirectToAction(nameof(HomeController.SignUp));
            }

           
            ModelState.AddModelErrorList(identityResult.Errors.Select(e=>e.Description).ToList());

            return View();
        }


        public IActionResult ForgotPassword()
        {
          
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                ModelState.AddModelError(String.Empty, "Bu E-Postaya Sahip kullanıcı bulunamadı");
                return View();
            }

            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // bir link üretmek lazım
            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = user.Id, token = passwordResetToken });
            //  https://localhost:7006? userId =12345& token =a sdad123asd3

            //Email Servis Lazım

            TempData["SuccesMessage"] = "Şifre yenileme linki e-posta adresinize gönderildi";
            return RedirectToAction(nameof(ForgotPassword));
            // vmmk sjtq swlv kali
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}
