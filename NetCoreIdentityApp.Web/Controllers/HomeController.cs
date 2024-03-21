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
        public async Task<IActionResult> SignIn(SignInViewModel request, string? returnUurl = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            returnUurl = returnUurl ?? Url.Action("Index", "Home");
            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            if (hasUser == null)
            {

                ModelState.AddModelError(string.Empty, "E-posta ya da şifre hatalı.");
                return View(request);
            }

            var signInResult = await _signInManager.PasswordSignInAsync(
                hasUser, 
                request.Password, 
                isPersistent: request.RememberMe, 
                lockoutOnFailure: false);


            if (signInResult.Succeeded)
            {
                return Redirect(returnUurl);
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



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
