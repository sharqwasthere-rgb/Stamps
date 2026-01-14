using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Stamps.Web.Services;

public class GeocodeService : IGeocodeService
{
    private readonly HttpClient _httpClient;

    public GeocodeService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "StampsApp/1.0");
    }

    public async Task<List<AddressSuggestion>> SearchAddressAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
        {
            return new List<AddressSuggestion>();
        }

        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<NominatimResult>>(
                $"search?q={Uri.EscapeDataString(query)}&format=json&addressdetails=1&limit=5"
            );

            if (response == null)
            {
                return new List<AddressSuggestion>();
            }

            return response.Select(r => new AddressSuggestion
            {
                DisplayName = r.DisplayName,
                Road = r.Address?.Road ?? r.Address?.Street ?? "",
                City = r.Address?.City ?? r.Address?.Town ?? r.Address?.Village ?? "",
                State = r.Address?.State ?? r.Address?.Province ?? "",
                PostalCode = r.Address?.Postcode ?? "",
                Country = r.Address?.Country ?? "",
                Latitude = double.TryParse(r.Lat, out var lat) ? lat : 0,
                Longitude = double.TryParse(r.Lon, out var lon) ? lon : 0
            }).ToList();
        }
        catch
        {
            return new List<AddressSuggestion>();
        }
    }

    private class NominatimResult
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("lat")]
        public string Lat { get; set; } = string.Empty;

        [JsonPropertyName("lon")]
        public string Lon { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public NominatimAddress? Address { get; set; }
    }

    private class NominatimAddress
    {
        [JsonPropertyName("road")]
        public string? Road { get; set; }

        [JsonPropertyName("street")]
        public string? Street { get; set; }

        [JsonPropertyName("house_number")]
        public string? HouseNumber { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("town")]
        public string? Town { get; set; }

        [JsonPropertyName("village")]
        public string? Village { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("province")]
        public string? Province { get; set; }

        [JsonPropertyName("postcode")]
        public string? Postcode { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }
}

