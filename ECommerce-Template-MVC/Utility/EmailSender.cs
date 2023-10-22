using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace ECommerce_Template_MVC.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse("test@gmail.com"));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate("roberto.developpeur@gmail.com", "vumcwacncvfkullq"); //nuestra cuenta
            smtp.Send(emailToSend); //methode envoyer courriel
            smtp.Disconnect(true);

            return Task.CompletedTask;
        }
    }
}
