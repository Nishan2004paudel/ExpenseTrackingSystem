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
            var verificationLink = $"http://localhost:4200/verify-email?token={Uri.EscapeDataString(verificationToken)}";
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

        public async Task SendPasswordResetEmailAsync(string toEmail, string fullName, string resetToken)
        {
            var resetLink = $"http://localhost:4200/reset-password?token={Uri.EscapeDataString(resetToken)}";
            var subject = "Reset your Expense Tracker password";
            var body = $@"
                <html>
                    <body style='font-family:Arial,sans-serif;'>
                    <h2>Hello {fullName}, </h2>
                    <p>We received a request to reset the password for your Expense Tracker account.</p>

                    <p>
                    Click the button below to create a new password.
                    </p>
                    <p style = 'margin:30px 0'>
                    <a href='{resetLink}'
                      style ='background:#2563eb;
                    color:white;
                    padding:12px 20px;
                    text-decoration:none;
                    border-radius:6px;'>
                    
                    Reset Password
                    </a>
                    </p>

                    <p>
                       If the button doesn't work, copy and paste this link:
                    </p>
                    <p>{resetLink}</p>
                    <p>
                        If you didn't request this password reset, you can safely ignore this email.
                    </p>
                    
                    <p>
                        This link expires in <b>1 hour</b>.
                    </p>

                    </body>
                 </html>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}
