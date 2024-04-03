using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NetCoreIdentityApp.Web.Models;
using System.Text;

namespace NetCoreIdentityApp.Web.TagHelpers
{
    public class UserRoleNamesTagHelper : TagHelper
    {

        public string UserId { get; set; }

        private readonly UserManager<AppUser> _userManager;

        public UserRoleNamesTagHelper(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            var user = await _userManager.FindByIdAsync(UserId); // kullanıcı bul

            var userRoles = await _userManager.GetRolesAsync(user!);// kullanıcının rollerrini getir bana liste olarak getir

            var stringBuilder = new StringBuilder(); // stringleri yan yana ekleyeceğiz

            userRoles.ToList().ForEach(x => // kullanıcı rollerinde dön ve html'e ekle
            {
                stringBuilder.Append(@$"<span class='badge bg-secondary mx-1'>{x.ToLower()}</span>");
            });

            output.Content.SetHtmlContent(stringBuilder.ToString());

        }
    }
}
