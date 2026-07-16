using expensetrackerserver.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
namespace expensetrackerserver.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(
                _settings.SenderName,
                _settings.SenderEmail));

            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new BodyBuilder
            {
                HtmlBody = htmlBody
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(_settings.Host,
                _settings.Port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(_settings.Username,
                _settings.Password);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
        public async Task SendVerificationEmailAsync(string toEmail, string fullName, string verificationToken)
        {
            var verificationLink = $"http://localhost:4200/verify-email?token={verificationToken}";
            var subject = "Verify your Expense Tracker account";
            var htmlBody = $@"
                <h2>Welcome, {fullName}!</h2>
                <p>Thank you for registering.</p>
                <p>Please verify your email by clicking the button below.</p>
                <p>
                    <a href=""{verificationLink}"">
                        Verify Email
                    </a>
                </p>
                <p>This link expires in 24 hours.</p>
                    ";
            await SendEmailAsync(
                toEmail,
                    subject,
                    htmlBody);
        }
    }
}
