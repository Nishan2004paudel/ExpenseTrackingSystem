namespace expensetrackerserver.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlBody);

        Task SendVerificationEmailAsync(string toEmail, string fullName, string verificationToken);
    }
}
