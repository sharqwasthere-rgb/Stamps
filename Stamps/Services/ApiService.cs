using System.Net.Http.Json;
using System.Text.Json;
using Stamps.Shared.Services;

namespace Stamps.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://stamps-ecxm.onrender.com"; // Render deployment

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    public async Task<ApiResponse<AuthResponse>> RegisterAsync(Shared.Services.RegisterRequest request)
    {
        try
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request, jsonOptions);
            
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return new ApiResponse<AuthResponse>
                {
                    Success = true,
                    Data = data
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error response: {errorContent}");
                var friendlyMessage = GetErrorMessageFromBody(errorContent, response.StatusCode);
                return new ApiResponse<AuthResponse>
                {
                    Success = false,
                    ErrorMessage = friendlyMessage
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection error: {ex}");
            return new ApiResponse<AuthResponse>
            {
                Success = false,
                ErrorMessage = $"Connection error: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(Shared.Services.LoginRequest request)
    {
        try
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request, jsonOptions);
            
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<AuthResponse>();
                return new ApiResponse<AuthResponse>
                {
                    Success = true,
                    Data = data
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                var friendlyMessage = GetErrorMessageFromBody(errorContent, response.StatusCode);
                return new ApiResponse<AuthResponse>
                {
                    Success = false,
                    ErrorMessage = friendlyMessage ?? "Login failed."
                };
            }
        }
        catch (Exception ex)
        {
            return new ApiResponse<AuthResponse>
            {
                Success = false,
                ErrorMessage = $"Connection error: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Parses API error body: either { "message": "..." } or ProblemDetails (title, errors, detail).
    /// </summary>
    private static string GetErrorMessageFromBody(string errorContent, System.Net.HttpStatusCode statusCode)
    {
        if (string.IsNullOrWhiteSpace(errorContent))
            return $"Request failed ({(int)statusCode}).";
        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        try
        {
            using var doc = JsonDocument.Parse(errorContent);
            var root = doc.RootElement;
            // Our API returns { "message": "..." }
            if (root.TryGetProperty("message", out var msg))
                return msg.GetString() ?? errorContent;
            // ProblemDetails: "title" and optionally "errors" or "detail"
            if (root.TryGetProperty("title", out var title))
            {
                var titleStr = title.GetString();
                if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in errors.EnumerateObject())
                        foreach (var val in prop.Value.EnumerateArray())
                        {
                            var s = val.GetString();
                            if (!string.IsNullOrWhiteSpace(s))
                                return $"{titleStr}: {s}";
                        }
                }
                if (root.TryGetProperty("detail", out var detail))
                {
                    var d = detail.GetString();
                    if (!string.IsNullOrWhiteSpace(d)) return d;
                }
                return titleStr ?? errorContent;
            }
        }
        catch { }
        return errorContent.Length > 200 ? errorContent[..200] + "…" : errorContent;
    }

    private class ErrorResponse
    {
        public string? Message { get; set; }
    }
}

