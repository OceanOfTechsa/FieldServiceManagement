using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.Areas.Identity.Models
{
    public class LoginWith2FAViewModel
    {
        [Required]
        [Display(Name = "Authenticator Code")]
        public string TwoFactorCode { get; set; } = string.Empty;

        public bool RememberMe { get; set; }

        public bool RememberMachine { get; set; }

        public string? ReturnUrl { get; set; }

        public string? Browser { get; set; }

        public bool IsInDev { get; set; }
    }
}
