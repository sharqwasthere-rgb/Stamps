using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using Stamps.Web.Data;

namespace Stamps.Web.Services;

public class QRCodeService : IQRCodeService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public QRCodeService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // Generate permanent QR code data for customer (this is their ID - never expires)
    public string GenerateUserQRCodeData(string userId, string userName)
    {
        var qrData = new
        {
            Type = "CustomerID",
            UserId = userId,
            UserName = userName,
            Generated = DateTime.UtcNow.ToString("o")
        };
        
        return JsonSerializer.Serialize(qrData);
    }

    // Decode customer QR code to get user ID
    public async Task<string?> DecodeUserQRCode(string qrData)
    {
        try
        {
            var data = JsonSerializer.Deserialize<QRCustomerData>(qrData);
            if (data?.Type == "CustomerID" && !string.IsNullOrEmpty(data.UserId))
            {
                // Verify user exists in database
                var userExists = await _context.Users.AnyAsync(u => u.Id == data.UserId);
                return userExists ? data.UserId : null;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    // Generate QR code image from any data
    public byte[] GenerateQRCodeImage(string data)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(20);
    }

    // Generate QR code image as base64 string
    public string GenerateUserQRCodeImage(string data)
    {
        var imageBytes = GenerateQRCodeImage(data);
        return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
    }

    // Generate temporary token (for web app compatibility)
    public async Task<string> CreateTokenAsync(string userId, QRTokenType type, int? stampCardId = null)
    {
        var token = GenerateSecureToken();
        var expirationMinutes = _configuration.GetValue<int>("QRCode:TokenExpirationMinutes", 5);

        var qrToken = new QRToken
        {
            UserId = userId,
            Token = token,
            Type = type,
            StampCardId = stampCardId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes)
        };

        _context.QRTokens.Add(qrToken);
        await _context.SaveChangesAsync();

        return token;
    }

    // Validate any token
    public async Task<QRToken?> ValidateTokenAsync(string token)
    {
        var qrToken = await _context.QRTokens
            .Include(qt => qt.User)
            .Include(qt => qt.StampCard)
            .FirstOrDefaultAsync(qt => qt.Token == token);

        if (qrToken == null || !qrToken.IsValid)
        {
            return null;
        }

        return qrToken;
    }

    // Mark token as used
    public async Task MarkTokenAsUsedAsync(string token)
    {
        var qrToken = await _context.QRTokens.FirstOrDefaultAsync(q => q.Token == token);
        if (qrToken != null)
        {
            qrToken.IsUsed = true;
            qrToken.UsedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    private string GenerateSecureToken()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[32];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private class QRCustomerData
    {
        public string? Type { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Generated { get; set; }
    }
}
