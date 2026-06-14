namespace FieldServiceManagement.Areas.Identity.Models
{
    public class TwoFactorAuthenticationViewModel
    {
        public bool Is2FaEnabled { get; set; }

        public bool HasAuthenticator { get; set; }

        public int RecoveryCodesLeft { get; set; }
    }
}
