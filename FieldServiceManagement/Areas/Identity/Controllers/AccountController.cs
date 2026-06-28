using FieldServiceManagement.Areas.Identity.Models;
using FieldServiceManagement.Business.NotificationBusiness.IdentityNotifications;
using FieldServiceManagement.Business.URLEncryptionBusiness;
using FieldServiceManagement.Data.DataModels.User;
using FieldServiceManagement.Enum;
using FieldServiceManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FieldServiceManagement.Web.Areas.Identity.Controllers;

[Area("Identity")]
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment env)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _env = env;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home", new { area = "" });

        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl,
            Browser = GetBrowser(),
            IsInDev = GetEnv()
        };

        return View(model);
    }


    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        model.Browser = GetBrowser();
        model.IsInDev = GetEnv();

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true
            );

            if (result.RequiresTwoFactor)
                return RedirectToAction(nameof(LoginWith2FA), new
                {
                    rememberMe = model.RememberMe,
                    returnUrl = model.ReturnUrl
                });
            var guardResult = ApplyGuards(result, model.ReturnUrl);
            if (guardResult != null)
                return guardResult;

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }
        catch (InvalidLoginException exception)
        {
            return exception.Reason switch
            {
                InvalidLoginReason.AccountInactive
                    => RedirectToAction("Suspended", "Account"),
                InvalidLoginReason.ProfileNotFound or
                InvalidLoginReason.InvalidRole
                    => RedirectToAction("LoginError", "Account", new { reason = exception.Reason }),
                _ => RedirectToAction("LoginError", "Account")
            };
        }
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> DeveloperLogin(string role)
    {
        if (!_env.IsDevelopment())
            return RedirectToAction(nameof(AccessDenied), "Account", new { area = "Identity", returnUrl = Request.Path });

        role = NormaliseUserRole(role);
        var result = await LogUserIn(MapRoleToEmail(role), role);
        if (result != null)
            return result;

        return RedirectToAction("Index", "Home", new { area = "" });
    }

    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home", new { area = "" });

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return model.CalledFromBusiness ? BadRequest(ModelState) : View(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            OrganisationId = Guid.NewGuid()
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            if (result.Errors.Any(e => e.Code == "DuplicateUserName" || e.Code == "DuplicateEmail"))
            {
                _ = Task.Run(() => new DuplicateRegistrationNotification(model.Email).SendNotification());
                string message = "If this email isn't already registered, your account has been created. Please check your inbox to continue..";

                if (model.CalledFromBusiness)
                    return Conflict(new { Succeeded = false, Message = message });

                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }
            return RegistrationFailure(model, result.Errors);
        }

        if (model.CalledFromBusiness)
            return Ok(new { Succeeded = true, Email = model.Email });

        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { 
            token = await _userManager.GenerateEmailConfirmationTokenAsync(user), 
            email = model.Email
        }, protocol: Request.Scheme);

        if (_env.IsDevelopment())
        {
            ViewBag.DevConfirmationLink = confirmationLink;
            return View(model);
        }

        _ = Task.Run(() => new WelcomeNotification(model.Email, confirmationLink!).SendNotificationWithoutQueue());
        return RedirectToAction("RegistrationConfirmation", "Account", new { token = UrlEncryptionBusiness.EncryptParam(model.Email) });
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string token, string email)
    {
        if (email == null || token == null)
            return RedirectToAction("Login", "Account");

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return NotFound();

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded ? RedirectToAction("Index", "Onboarding", new {token = UrlEncryptionBusiness.EncryptParam(email) }) : View("Error");
    }

    public async Task<IActionResult> ConfirmEmailSuccess() => View();


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account", new { area = "Identity" });
    }


    [AllowAnonymous]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        string? resetLink = null;
        if (user != null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            resetLink = Url.Action(nameof(ResetPassword), "Account",
                new { area = "Identity", token, email = model.Email },
                Request.Scheme);
        }

        if (_env.IsDevelopment())
        {
            ViewBag.DevResetLink = resetLink ?? "User not found (hidden in production)";
            return View(model); 
        }

        return RedirectToAction(nameof(ForgotPasswordConfirmation));
    }

    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation() => View();


    [AllowAnonymous]
    public IActionResult ResetPassword(string? token, string? email)
    {
        if (token is null || email is null)
            return RedirectToAction(nameof(Login));

        return View(new ResetPasswordViewModel { Token = token, Email = email });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
            return RedirectToAction(nameof(Login));

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

        if (result.Succeeded)
            return RedirectToAction(nameof(Login));

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> EmailConfirmation(string? token = null)
    {
        return View();
    }


    [AllowAnonymous]
    public IActionResult AccessDenied(string? returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    public IActionResult Developerlogin()
    {
        if (!_env.IsDevelopment())
            return RedirectToAction(nameof(AccessDenied), "Account", new { area = "Identity", returnUrl = Request.Path });

        return View();
    }

    [AllowAnonymous, MVCDecryptFilter]
    public IActionResult RegistrationConfirmation(string token)
    {
        return View((object)token);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Lockout() => View();

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Suspended() => View();

    [HttpGet]
    [AllowAnonymous]
    public IActionResult LoginError(InvalidLoginReason? reason) => View(reason);

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LoginWith2FA(bool rememberMe = false, string? returnUrl = null)
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

        if (user == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var model = new LoginWith2FAViewModel
        {
            RememberMe = rememberMe,
            ReturnUrl = returnUrl
        };

        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginWith2FA(LoginWith2FAViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var code = model.TwoFactorCode
            .Replace(" ", string.Empty)
            .Replace("-", string.Empty);

        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
            code,
            model.RememberMe,
            model.RememberMachine
        );

        var guardResult = ApplyGuards(result, model.ReturnUrl);
        if (guardResult != null)
            return guardResult;

        ModelState.AddModelError(string.Empty, "Invalid authentication code.");
        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LoginWithRecoveryCode()
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
            return RedirectToAction("Login", "Account", new { error = "Session expired. Please log in again." });

        return View(new LoginWithRecoveryCode());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCode model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Your 2FA session expired. Please re-enter your email and password.");
            return View(model);
        }
        var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(model.RecoveryCode);
        var guardResult = ApplyGuards(result, null);
        if (guardResult != null)
            return guardResult;

        ModelState.AddModelError(string.Empty, "Invalid recovery code. Please check the code and try again.");
        return View(model);
    }

    #region PPRIVATE METHODS
    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home", new { area = "" });
    }

    private async Task<IActionResult?> LogUserIn(string email, string role)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return RedirectToAction("Login", "Account", new { area = "Identity" });

        var claims = await _userManager.GetClaimsAsync(user);
        var roleClaims = claims.Where(x => x.Type == ClaimTypes.Role).ToList();

        if (!roleClaims.Any())
        {
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role));
        }
        else if (roleClaims.All(x => x.Value != role))
        {
            await _userManager.RemoveClaimsAsync(user, roleClaims);
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role));
        }
        await _signInManager.SignInAsync(user, isPersistent: false);
        return null;
    }
    private static string MapRoleToEmail(string role)
    {
        var email = role switch
        {
            "Administrator" => "administrator@fsm.oceanoftech.co.za",
            "Dispatcher" => "dispatcher@fsm.oceanoftech.co.za",
            "FieldAgent" => "fa@fsm.oceanoftech.co.za",
            "CallCenterAgent" => "cca@fsm.oceanoftech.co.za",
            "LimitedFieldAgent" => "lfa@fsm.oceanoftech.co.za",
            "SuperAdmin" => "system@fsm.com",
            _ => role + "@fsm.oceanoftech.co.za"
        };
        return email;
    }

    private static string NormaliseUserRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return role; 
        var roleMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "DebtorsInquiryDesk2", "DebtorsInquiryDesk" },
        };
        return roleMap.TryGetValue(role, out var normalizedRole) ? normalizedRole : role;
    }
    private string GetBrowser()
    {
        return Request.Headers.UserAgent.ToString();
    }
    private bool GetEnv()
    {
        return _env.IsDevelopment();
    }

    private IActionResult RegistrationFailure(RegisterViewModel model, IEnumerable<IdentityError> errors)
    {
        foreach (var error in errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return model.CalledFromBusiness
            ? BadRequest(new { Succeeded = false, Errors = errors.Select(e => e.Description) })
            : View(model);
    }

    private IActionResult? ApplyGuards(Microsoft.AspNetCore.Identity.SignInResult result, string? returnUrl)
    {
        if (result.Succeeded)
            return RedirectToLocal(returnUrl);

        if (result.IsLockedOut)
            return RedirectToAction(nameof(Lockout));

        return null;
    }
    #endregion
}