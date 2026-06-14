using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.ViewModels.User
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DisplayName("Current Password")]
        public string CurrentPassword { get; set; }

        [Required]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DisplayName("Confirm New Password")]
        public string ConfirmPassword { get; set; }
    }
}
