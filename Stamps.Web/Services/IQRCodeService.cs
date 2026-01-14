using Stamps.Web.Data;

namespace Stamps.Web.Services;

public interface IQRCodeService
{
    // Generate permanent QR code data for a customer (contains user ID)
    string GenerateUserQRCodeData(string userId, string userName);
    
    // Decode customer QR code to get user ID
    Task<string?> DecodeUserQRCode(string qrData);
    
    // Generate QR code image from data
    byte[] GenerateQRCodeImage(string data);
    
    // Generate QR code image as base64 string
    string GenerateUserQRCodeImage(string data);
    
    // Generate temporary token (for web app compatibility)
    Task<string> CreateTokenAsync(string userId, QRTokenType type, int? stampCardId = null);
    
    // Validate any token
    Task<QRToken?> ValidateTokenAsync(string token);
    
    // Mark token as used
    Task MarkTokenAsUsedAsync(string token);
}
