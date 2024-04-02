using System.ComponentModel.DataAnnotations;

namespace NetCoreIdentityApp.Web.Areas.Admin.Models
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Role isim alanı boş bırakılamaz.")]
        [Display(Name = "Role ismi :")]
        public string Name{ get; set; }
    }
}
