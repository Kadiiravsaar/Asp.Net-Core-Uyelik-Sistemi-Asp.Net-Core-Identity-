using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCoreIdentityApp.Web.Areas.Admin.Models;
using NetCoreIdentityApp.Web.Extensions;
using NetCoreIdentityApp.Web.Models;

namespace NetCoreIdentityApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RolesController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            //var roles = _roleManager.Roles.ToList();

            var roles = await _roleManager.Roles.Select(x => new RoleViewModel() // rolleri çek ve her birini RoleViewModel'e dönüştür
            {
                Id = x.Id, // x = AppRole
                Name = x.Name
            }).ToListAsync();

            return View(roles);
        }
        [HttpGet]
        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(CreateRoleViewModel request)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = request.Name });

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();

            }
            return RedirectToAction(nameof(RolesController.Index));
        }

        [HttpGet]
        public async Task<IActionResult> RoleUpdate(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);
            if (roleToUpdate == null)
            {
                throw new Exception("Böyle bir rol yoktur");
            }

            return View(new RoleUpdateViewModel() { Id = id,Name = roleToUpdate.Name});
        }



        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
        {

            var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);
            if (roleToUpdate == null)
            {
                throw new Exception("Böyle bir rol yoktur");
            }
            roleToUpdate.Name = request.Name;
            await _roleManager.UpdateAsync(roleToUpdate);

            ViewData["SuccessMessage"] = "Rol bilgisi Güncellenmiştir";
            return View();
        }



        public async Task<IActionResult> RoleDelete(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);

            if (roleToDelete == null)
            {
                throw new Exception("Silinecek rol bulunamamıştır.");
            }

            var result = await _roleManager.DeleteAsync(roleToDelete);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.Select(x => x.Description).First());
            }

            TempData["SuccessMessage"] = "Rol silinmiştir";
            return RedirectToAction(nameof(RolesController.Index));




        }

        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            var currentUser = (await _userManager.FindByIdAsync(id))!; // kullanıcı bulalım
           var roles = await _roleManager.Roles.ToListAsync(); // var olan tüm rolleri bulalım

            var userRoles = await _userManager.GetRolesAsync(currentUser); // kullanıcının rolleri liste string olarak beriyor

            var roleViewModelList = new List<AssignRoleToUserViewModel>(); // liste olarak atama yaptım

            foreach (var role in roles) // mevcut Rollerde dönelim
            {

                var assignRoleToUserViewModel = new AssignRoleToUserViewModel() { Id = role.Id, Name = role.Name! };
                // her döndüğümde bir  nesne örneği oluşturucam


                if (userRoles.Contains(role.Name!)) // eğer kullanıcı rollerinde var ise yokas aşağıad listeye eklettim
                {
                    assignRoleToUserViewModel.Exist = true; // var mı yok mu ona bakıcam
                }

                roleViewModelList.Add(assignRoleToUserViewModel);


            }

            return View(roleViewModelList);
        }

       
    }
}

