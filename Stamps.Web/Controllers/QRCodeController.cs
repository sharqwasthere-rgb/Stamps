using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stamps.Web.Data;
using Stamps.Web.Services;

namespace Stamps.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QRCodeController : ControllerBase
{
    private readonly IQRCodeService _qrCodeService;
    private readonly UserManager<ApplicationUser> _userManager;

    public QRCodeController(IQRCodeService qrCodeService, UserManager<ApplicationUser> userManager)
    {
        _qrCodeService = qrCodeService;
        _userManager = userManager;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserQRCode(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Generate permanent QR code data for this user
        var qrData = _qrCodeService.GenerateUserQRCodeData(user.Id, user.FullName);
        
        // Generate QR code image
        var qrImage = _qrCodeService.GenerateQRCodeImage(qrData);
        var base64Image = Convert.ToBase64String(qrImage);

        return Ok(new
        {
            userId = user.Id,
            userName = user.FullName,
            qrData = qrData,
            qrImage = $"data:image/png;base64,{base64Image}"
        });
    }

    [HttpPost("scan")]
    public async Task<IActionResult> ScanCustomerQRCode([FromBody] ScanQRRequest request)
    {
        if (string.IsNullOrEmpty(request.QRData))
        {
            return BadRequest(new { message = "QR code data is required" });
        }

        // Decode the QR code to get user ID
        var userId = await _qrCodeService.DecodeUserQRCode(request.QRData);
        
        if (userId == null)
        {
            return BadRequest(new { message = "Invalid QR code" });
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(new
        {
            userId = user.Id,
            userName = user.FullName,
            email = user.Email,
            userType = user.UserType.ToString()
        });
    }
}

public class ScanQRRequest
{
    public string QRData { get; set; } = string.Empty;
}

