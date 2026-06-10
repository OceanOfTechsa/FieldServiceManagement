using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.Areas.Identity.Models;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Email Required")]
    [EmailAddress(ErrorMessage = "Please provide a valide email address")]
    public string Email { get; set; } = string.Empty;
}