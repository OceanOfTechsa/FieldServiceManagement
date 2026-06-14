using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.Areas.Identity.Models
{
    public class LoginWithRecoveryCode
    {
        [Required]
        [Display(Name = "Recovery Code")]
        [Length(11, 11, ErrorMessage = "Recovery code must be in the format XXXXX-XXXXX.")]
        public string RecoveryCode { get; set; } = string.Empty;
    }
}
