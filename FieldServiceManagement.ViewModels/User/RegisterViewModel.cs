using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FieldServiceManagement.Areas.Identity.Models;

public class RegisterViewModel
{

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "Accept Terms and Conditions")]
    public bool AcceptTerms { get; set; }
    public bool IsInDev { get; set; } = false;



    /// <summary>
    /// When true, bypasses view returns and returns a result object instead.
    /// </summary>
    [JsonIgnore]
    public bool CalledFromBusiness { get; set; } = false;
}