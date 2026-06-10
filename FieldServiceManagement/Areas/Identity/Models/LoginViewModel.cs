using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.Areas.Identity.Models;

public class LoginViewModel
{
    
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }

    public string Browser { get; set; } = "";

    public bool IsInDev { get; set; } = false;
}