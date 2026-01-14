using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Stamps.Web.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailVerificationAsync(string email, string fullName, string verificationToken)
    {
        var subject = "Verify Your Email - Stamps";
        var body = $@"
            <h2>Welcome to Stamps, {fullName}!</h2>
            <p>Please verify your email address by clicking the link below:</p>
            <p><a href='{_configuration["AppUrl"]}/Account/VerifyEmail?token={verificationToken}'>Verify Email</a></p>
            <p>This link will expire in 24 hours.</p>
            <p>If you didn't create this account, please ignore this email.</p>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetAsync(string email, string fullName, string resetToken)
    {
        var subject = "Reset Your Password - Stamps";
        var body = $@"
            <h2>Password Reset Request</h2>
            <p>Hi {fullName},</p>
            <p>We received a request to reset your password. Click the link below to create a new password:</p>
            <p><a href='{_configuration["AppUrl"]}/Account/ResetPassword?token={resetToken}'>Reset Password</a></p>
            <p>This link will expire in 1 hour.</p>
            <p>If you didn't request this, please ignore this email and your password will remain unchanged.</p>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string email, string fullName)
    {
        var subject = "Welcome to Stamps!";
        var body = $@"
            <h2>Welcome to Stamps, {fullName}!</h2>
            <p>Thank you for joining us. Start collecting stamps and earning rewards today!</p>
            <p>Download our app and start earning rewards at your favorite stores.</p>
        ";

        await SendEmailAsync(email, subject, body);
    }

    private async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPass = _configuration["Email:SmtpPassword"];
            var fromEmail = _configuration["Email:FromEmail"];
            var fromName = _configuration["Email:FromName"] ?? "Stamps";

            // Skip if email not configured (development mode)
            if (string.IsNullOrEmpty(smtpHost))
            {
                _logger.LogWarning("Email not configured. Would have sent to {Email}: {Subject}", to, subject);
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            
            if (!string.IsNullOrEmpty(smtpUser))
            {
                await client.AuthenticateAsync(smtpUser, smtpPass);
            }
            
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {Email}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", to);
            // Don't throw - email failures shouldn't break the app
        }
    }
}

