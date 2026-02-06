using System.Net.Mail;

namespace AlumniApi.Services.Email
{
   
    public interface IEmailSender
    {
        Task SendAsync(EmailMessage message, CancellationToken ct = default);
    }

}

