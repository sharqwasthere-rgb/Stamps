namespace Stamps.Shared.Services;

public interface IEmailValidationService
{
    Task<EmailValidationResult> ValidateEmailAsync(string email);
}

public class EmailValidationResult
{
    public bool IsValid { get; set; }
    public bool IsDisposable { get; set; }
    public string? SuggestedEmail { get; set; }
    public string? ErrorMessage { get; set; }
}

