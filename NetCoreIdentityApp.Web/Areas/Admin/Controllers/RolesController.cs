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

    }
}
