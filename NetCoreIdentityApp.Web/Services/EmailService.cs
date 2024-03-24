
using Microsoft.Extensions.Options;
using NetCoreIdentityApp.Web.OptionsModels;
using System.Net;
using System.Net.Http;
using System.Net.Mail;

namespace NetCoreIdentityApp.Web.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendResetPasswordEmail(string resetPasswordEmailLink, string toEmail)
        {    
            var smtpClient = new SmtpClient();
            smtpClient.Host = _emailSettings.Host;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);
            smtpClient.EnableSsl =true;



            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(toEmail);
            mailMessage.To.Add(toEmail);
            mailMessage.Subject = "Localhost şifre sıfırlama linki";
            mailMessage.Body = @$"
                  <h4>Şifrenizi yenilemek için aşağıdaki linkte tıklayınız.</h4>
                  <p><a href='{resetPasswordEmailLink}'>şifre yenileme link</a></p>";

            mailMessage.IsBodyHtml = true;


            await smtpClient.SendMailAsync(mailMessage);

        }
    }
}
