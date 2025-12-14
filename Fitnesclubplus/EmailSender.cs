using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace Fitnesclubplus
{
    // Bu sınıf, mail atıyormuş gibi yapar ama aslında hiçbir şey yapmaz.
    // Böylece hata almaktan kurtuluruz.
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Burası boş, işlem başarılıymış gibi geri dönüyor.
            return Task.CompletedTask;
        }
    }
}