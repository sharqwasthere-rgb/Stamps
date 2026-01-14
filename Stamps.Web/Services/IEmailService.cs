namespace Stamps.Web.Services;

public interface IEmailService
{
    Task SendEmailVerificationAsync(string email, string fullName, string verificationToken);
    Task SendPasswordResetAsync(string email, string fullName, string resetToken);
    Task SendWelcomeEmailAsync(string email, string fullName);
}

