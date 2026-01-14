using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stamps.Web.Data;

namespace Stamps.Web.Controllers;

[Route("Account")]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet("ExternalLogin")]
    public IActionResult ExternalLogin(string provider, string returnUrl = "/")
    {
        var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet("ExternalLoginCallback")]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/", string? remoteError = null)
    {
        if (remoteError != null)
        {
            return RedirectToAction("Login", new { error = $"Error from external provider: {remoteError}" });
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction("Login", new { error = "Error loading external login information." });
        }

        var signInResult = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (signInResult.Succeeded)
        {
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user != null)
            {
                if (user.UserType == UserType.Client)
                {
                    return Redirect("/Client/Dashboard");
                }
                else
                {
                    return Redirect("/Store/Dashboard");
                }
            }
            return LocalRedirect(returnUrl);
        }

        if (signInResult.IsLockedOut)
        {
            return RedirectToAction("Login", new { error = "Account is locked out." });
        }

        // User doesn't have an account, create one
        var email = info.Principal.FindFirstValue(System.Security.Claims.ClaimTypes.Email);
        var name = info.Principal.FindFirstValue(System.Security.Claims.ClaimTypes.Name);

        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", new { error = "Email not provided by external provider." });
        }

        // Default to Client, user can change later if needed
        var newUser = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = name ?? email,
            UserType = UserType.Client
        };

        var createResult = await _userManager.CreateAsync(newUser);
        if (createResult.Succeeded)
        {
            await _userManager.AddLoginAsync(newUser, info);
            await _signInManager.SignInAsync(newUser, isPersistent: false);

            return Redirect("/Client/Dashboard");
        }

        return RedirectToAction("Login", new { error = "Failed to create account." });
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Redirect("/");
    }
}

