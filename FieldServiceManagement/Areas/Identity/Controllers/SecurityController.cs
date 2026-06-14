using FieldServiceManagement.Areas.Identity.Models;
using FieldServiceManagement.Data.DataModels.User;
using FieldServiceManagement.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

[Area("Identity")]
[Authorize]
[Route("Identity/Account/Manage/Security")]
public class SecurityController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public SecurityController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("EnableAuthenticator")]
    public async Task<IActionResult> EnableAuthenticator()
    {
        var user = await _userManager.GetUserAsync(User);
        var key = await _userManager.GetAuthenticatorKeyAsync(user!);
        if (string.IsNullOrEmpty(key))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user!);
            key = await _userManager.GetAuthenticatorKeyAsync(user!);
        }

        var model = new EnableAuthenticatorViewModel
        {
            SharedKey = key!,
            AuthenticatorUri = GenerateQrCodeUri(user!.Email!, key!),
            TwoFactorEnabled = user.TwoFactorEnabled
        };

        return View(model);
    }


    [HttpGet("QrCode")]
    public IActionResult QrCode(string qrCodeUri)
    {
        if (string.IsNullOrEmpty(qrCodeUri))
            return BadRequest("qrCodeUri is required");

        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(data);

        return File(qrCode.GetGraphic(20), "image/png");
    }


    [HttpPost("EnableAuthenticator")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (!ModelState.IsValid)
        {
            var key = await _userManager.GetAuthenticatorKeyAsync(user!);
            model.SharedKey = key!;
            model.AuthenticatorUri = GenerateQrCodeUri(user!.Email!, key!);
            return View(model);
        }

        var verificationCode = model.VerificationCode
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty);

        var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(user!,
            _userManager.Options.Tokens.AuthenticatorTokenProvider,
            verificationCode);

        if (!is2faTokenValid)
        {
            return Json(new { success = false, error = "Invalid verification code, please try again." });
        }

        await _userManager.SetTwoFactorEnabledAsync(user!, true);
        return Json(new { success = true });
    }

    [HttpGet("Disable2FA")]
    public IActionResult Disable2FA() =>  View();
    

    [HttpPost("Disable2FA")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Disable2FAConfirmed()
    {
        var user = await _userManager.GetUserAsync(User);
        await _userManager.SetTwoFactorEnabledAsync(user!,false);
        await _signInManager.RefreshSignInAsync(user!);
        return RedirectToAction(nameof(EnableAuthenticator));
    }

    [HttpGet("ChangePassword")]
    public IActionResult ChangePassword() => View();

    [HttpPost("ChangePassword")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();

        var passwordValid =
            await _userManager.CheckPasswordAsync(user, model.CurrentPassword);

        if (!passwordValid)
        {
            ModelState.AddModelError(nameof(model.CurrentPassword),"The current password entered is incorrect.");
            return View(model);
        }

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    [HttpGet("RecoveryCodes")]
    public async Task<IActionResult> DownloadRecoveryCodes()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
            return Challenge();

        var codes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);

        if (codes == null || !codes.Any())
            return BadRequest("Could not generate recovery codes.");

        var content = string.Join(Environment.NewLine, codes);
        var fileName = $"OOT FSM-RecoveryCodes_{DateTime.UtcNow:yyyyMMdd}.txt";
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);

        return File(bytes, "text/plain", fileName);
    }

    private string GenerateQrCodeUri(string email, string unformattedKey)
    {
        return string.Format(
            "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
            "OOT FSM",
            email,
            unformattedKey);
    }
}