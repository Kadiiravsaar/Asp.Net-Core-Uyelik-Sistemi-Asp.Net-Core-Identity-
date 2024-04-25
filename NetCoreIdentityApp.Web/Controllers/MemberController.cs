using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using NetCoreIdentityApp.Web.Extensions;
using NetCoreIdentityApp.Web.Models;
using NetCoreIdentityApp.Web.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;

namespace NetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;

        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            var userViewModel = new UserViewModel
            {
                Email = currentUser!.Email,
                UserName = currentUser!.UserName,
                PhoneNumber = currentUser!.PhoneNumber,
                Picture = currentUser!.Picture
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
                ModelState.AddModelErrorList(resultChangePassword.Errors);
                return View();
            }

            TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirilmiştir.";

            await _userManager.UpdateSecurityStampAsync(user);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(user, request.PasswordNew, true, false);

            return View();
        }

        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var userEditViewModel = new UserEditViewModel()
            {

                UserName = user.UserName!,
                Email = user.Email!,
                Phone = user.PhoneNumber!,
                BirthDate = user.BirthDate,
                City = user.City,
                Gender = user.Gender
            };

            return View(userEditViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.FindByNameAsync(User.Identity!.Name!); // kullanıcı adına göre kullanıcı bilgilerini al

            user.UserName = request.UserName;
            user.Email = request.Email;
            user.PhoneNumber = request.Phone;
            user.BirthDate = request.BirthDate;
            user.City = request.City;
            user.Gender = request.Gender;

            if (request.Picture != null && request.Picture.Length > 0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot"); // nereye gitmek istiyorsam referans noktasını veriyorum

                string randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";// random foto ismi + jpg/png

                var newPicturePath = Path.Combine(wwwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath!, randomFileName); // wwwroot fiziksel adresini ver . ve vu işlem yeni yolu oluşturmaya yarar

                using var stream = new FileStream(newPicturePath, FileMode.Create); // yeni yolu kaydetmek için stream aç. Yeni dosya oluştur

                await request.Picture.CopyToAsync(stream); // dışarıdan (parametre) gelen picture kopyala ve yeni dosyanın içine kopyala

                user.Picture = randomFileName; // bu dosyanın yolu db'ye yazmak lazım

            }

            var updateToUserResult = await _userManager.UpdateAsync(user); // kullanıcıyı güncelle. Geriye ne dönüyor bak

            if (!updateToUserResult.Succeeded)
            {
                ModelState.AddModelErrorList(updateToUserResult.Errors);
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(user); // SecuritySecurityStamp güncelle
            await _signInManager.SignOutAsync(); // cokkie bilgisi için giriş çıkış yaptııyorum

            if (request.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(user, true, new[] { new Claim("birthdate", user.BirthDate!.Value.ToString()) });

            }
            else
            {
                await _signInManager.SignInAsync(user, true); // passwordümü alamadığım için elimde olmadığı için SignInAsync ile yaptım. Elimde olsaydı PasswordSignInAsync ile giriş yaptırırdım

            }

            TempData["SuccessMessage"] = "Üye bilgileri başarıyla değiştirilmiştir";

            var userEditViewModel = new UserEditViewModel()
            {
                UserName = user.UserName!,
                Email = user.Email!,
                Phone = user.PhoneNumber!,
                BirthDate = user.BirthDate,
                City = user.City,
                Gender = user.Gender,
            };

            return View(userEditViewModel);
        }

        [HttpGet]
        public IActionResult Claims()
        {
            //Select ifadesi hakkında daha fazla bilgi vermek gerekirse, LINQ (Language Integrated Query) ifadelerinin bir parçasıdır
            //ve bir koleksiyonun her bir öğesini belirli bir işleme tabi tutar.
            var userClaimList = User.Claims.Select(x => new ClaimViewModel()
            {
                Issuer = x.Issuer,
                Type = x.Type,
                Value = x.Value
            }).ToList();
            return View();
        }


        [Authorize(Policy = "AnkaraPolicy")]
        [HttpGet]
        public IActionResult AnkaraPage()
        {
         
            return View();
        }

        [Authorize(Policy = "ExchangePolicy")]
        [HttpGet]
        public IActionResult ExchangePage()
        {

            return View();
        }



        public IActionResult AccessDenied(string returnUrl)
        {
            string message = string.Empty;
            message = "Yetkiniz yoktur lürfen yetkili biri ile iletişime geçiniz";
            ViewBag.message = message;
            return View();
        }
    }
}
