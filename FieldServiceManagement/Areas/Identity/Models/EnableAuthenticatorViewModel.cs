using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.Areas.Identity.Models
{
    public class EnableAuthenticatorViewModel
    {
        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }

        [Required]
        public string VerificationCode { get; set; }

        public bool? TwoFactorEnabled { get; set; }
    }
}
