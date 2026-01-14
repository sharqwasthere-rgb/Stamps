namespace Stamps.Web.Services;

public interface IGeocodeService
{
    Task<List<AddressSuggestion>> SearchAddressAsync(string query);
}

public class AddressSuggestion
{
    public string DisplayName { get; set; } = string.Empty;
    public string Road { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

