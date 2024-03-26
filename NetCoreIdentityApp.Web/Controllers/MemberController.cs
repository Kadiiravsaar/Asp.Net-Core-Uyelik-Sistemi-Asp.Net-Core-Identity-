using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCoreIdentityApp.Web.Extensions;
using NetCoreIdentityApp.Web.Models;
using NetCoreIdentityApp.Web.ViewModels;

namespace NetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            var userViewModel = new UserViewModel
            {
                Email = currentUser!.Email,
                UserName = currentUser!.UserName,
                PhoneNumber = currentUser!.PhoneNumber
            };
            return View(userViewModel);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(User!.Identity!.Name!);
            var checkOldPassword = await _userManager.CheckPasswordAsync(user, user.PasswordHash);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(String.Empty, "Eski Şifreniz Yanlış.");
                return View();
            }
            var resultChangePassword = await _userManager.ChangePasswordAsync(user, request.PasswordOld, request.PasswordNew);

            if (!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors.Select(x=>x.Description).ToList());
                return View();
            }

            TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirilmiştir.";

            await _userManager.UpdateSecurityStampAsync(user);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(user, request.PasswordNew, true, false);

            return View();
        }
    }
}
