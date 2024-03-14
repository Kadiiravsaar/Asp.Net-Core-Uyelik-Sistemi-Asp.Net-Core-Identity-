using System.ComponentModel.DataAnnotations;

namespace NetCoreIdentityApp.Web.ViewModels
{
    public class SignUpViewModel
    {
        public SignUpViewModel()
        {
            
        }

        public SignUpViewModel(string userName, string email, string phoneNumber, string password)
        {
            UserName = userName;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
        }

        [Display(Name ="Kullanıcı Adı: ")]
        public string UserName { get; set; }


        [Display(Name = "Email: ")]
        public string Email { get; set; }


        [Display(Name ="Telefon: ")]
        public string PhoneNumber { get; set; }


        [Display(Name = "Şifre: ")]
        public string Password { get; set; }


        [Display(Name = "Şifre Tekrar: ")]
        public string PasswordConfirm { get; set; }
    }
}
