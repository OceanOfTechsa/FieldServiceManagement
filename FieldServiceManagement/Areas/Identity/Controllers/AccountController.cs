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

            if (result.Succeeded)
                return RedirectToLocal(model.ReturnUrl);

            if (result.IsLockedOut)
                return RedirectToAction("Lockout");

            ModelState.AddModelError("", "Invalid email or password.");
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
        {
            if (model.CalledFromBusiness)
                return BadRequest(ModelState);

            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            var confirmationLink = Url.Action("ConfirmEmail", "Account", new
            {
                token = await _userManager.GenerateEmailConfirmationTokenAsync(user),
                email = model.Email
            },
                protocol: Request.Scheme
            );

            if (model.CalledFromBusiness)
                return Ok(new { Succeeded = true, Email = model.Email });

            if (_env.IsDevelopment())
            {
                ViewBag.DevConfirmationLink = confirmationLink;
                return View(model);
            }
            else
            {
                new WelcomeNotification(model.Email, confirmationLink!).SendNotificationWithoutQueue();
                return RedirectToAction("RegistrationConfirmation", "Account",new { token = UrlEncryptionBusiness.EncryptParam(model.Email) });
            }
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        if (model.CalledFromBusiness)
            return BadRequest(new
            {
                Succeeded = false,
                Errors = result.Errors.Select(e => e.Description)
            });

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    //[MVCDecryptFilter]
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


    #region MANAGE

    [Route("Identity/Account/Manage")]
    public IActionResult Index()
    {
        return View();
    }
    #endregion


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
    #endregion
}