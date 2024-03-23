using System.ComponentModel.DataAnnotations;

namespace NetCoreIdentityApp.Web.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email alanı boş bırakılamaz.")]
        [EmailAddress(ErrorMessage = "Email formatı yanlıştır.")]
        [Display(Name = "Email ")]
        public string Email { get; set; } = null!;
    }
}
