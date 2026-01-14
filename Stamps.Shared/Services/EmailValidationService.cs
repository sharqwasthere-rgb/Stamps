using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace Stamps.Shared.Services;

public class EmailValidationService : IEmailValidationService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;

    public EmailValidationService(HttpClient httpClient, IConfiguration? configuration = null)
    {
        _httpClient = httpClient;
        _apiKey = configuration?["EmailValidation:AbstractApiKey"];
    }

    public async Task<EmailValidationResult> ValidateEmailAsync(string email)
    {
        // If no API key configured, skip API validation
        if (string.IsNullOrEmpty(_apiKey))
        {
            return new EmailValidationResult { IsValid = true };
        }

        try
        {
            // Using AbstractAPI Email Validation (100 free validations/month)
            // Sign up at: https://app.abstractapi.com/api/email-validation
            var response = await _httpClient.GetFromJsonAsync<AbstractApiResponse>(
                $"https://emailvalidation.abstractapi.com/v1/?api_key={_apiKey}&email={email}"
            );

            if (response == null)
            {
                return new EmailValidationResult { IsValid = true }; // Skip if API fails
            }

            return new EmailValidationResult
            {
                IsValid = response.Deliverability == "DELIVERABLE" && response.IsValidFormat?.Value == true,
                IsDisposable = response.IsDisposableEmail?.Value ?? false,
                SuggestedEmail = response.Autocorrect,
                ErrorMessage = !response.IsValidFormat?.Value ?? false 
                    ? "Invalid email format" 
                    : response.IsDisposableEmail?.Value ?? false 
                        ? "Disposable email addresses are not allowed" 
                        : null
            };
        }
        catch
        {
            // If API fails, don't block registration
            return new EmailValidationResult { IsValid = true };
        }
    }

    private class AbstractApiResponse
    {
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("autocorrect")]
        public string? Autocorrect { get; set; }

        [JsonPropertyName("deliverability")]
        public string? Deliverability { get; set; }

        [JsonPropertyName("is_valid_format")]
        public BooleanValue? IsValidFormat { get; set; }

        [JsonPropertyName("is_disposable_email")]
        public BooleanValue? IsDisposableEmail { get; set; }

        [JsonPropertyName("is_mx_found")]
        public BooleanValue? IsMxFound { get; set; }

        [JsonPropertyName("is_smtp_valid")]
        public BooleanValue? IsSmtpValid { get; set; }
    }

    private class BooleanValue
    {
        [JsonPropertyName("value")]
        public bool Value { get; set; }
    }
}

