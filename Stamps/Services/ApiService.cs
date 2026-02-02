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
                
                try
                {
                    var errorObj = JsonSerializer.Deserialize<ErrorResponse>(errorContent, new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    });
                    return new ApiResponse<AuthResponse>
                    {
                        Success = false,
                        ErrorMessage = errorObj?.Message ?? $"Registration failed. Status: {response.StatusCode}"
                    };
                }
                catch
                {
                    // If JSON deserialization fails, return the raw error content
                    return new ApiResponse<AuthResponse>
                    {
                        Success = false,
                        ErrorMessage = !string.IsNullOrWhiteSpace(errorContent) 
                            ? errorContent 
                            : $"Registration failed. Status: {response.StatusCode}"
                    };
                }
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
                var errorObj = JsonSerializer.Deserialize<ErrorResponse>(errorContent);
                return new ApiResponse<AuthResponse>
                {
                    Success = false,
                    ErrorMessage = errorObj?.Message ?? "Login failed."
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

    private class ErrorResponse
    {
        public string? Message { get; set; }
    }
}

