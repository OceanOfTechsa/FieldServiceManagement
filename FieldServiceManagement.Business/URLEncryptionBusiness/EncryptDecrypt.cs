using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace FieldServiceManagement.Business.URLEncryptionBusiness
{
    /// <summary>
    /// Version-aware encryption/decryption dispatcher.
    /// </summary>
    public class EncryptDecrypt : IEncryptDecrypt
    {
        private readonly string sharedSecret;

        private readonly LegacyEncryptDecrypt legacy;
        private readonly ModernEncryptDecrypt modern;

        private const string PrefixV2 = "v2|";

        public EncryptDecrypt(string sharedSecret)
        {
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException(nameof(sharedSecret));

            this.sharedSecret = sharedSecret;

            // instantiate correct handlers
            legacy = new LegacyEncryptDecrypt(sharedSecret);
            modern = new ModernEncryptDecrypt(sharedSecret);
        }

        // ============================================================================
        // EncryptString ALWAYS uses v2 (modern)
        // ============================================================================
        public string EncryptString(string value)
        {
            return modern.EncryptString(value);
        }

        // ============================================================================
        // DecryptString → detect version → route to correct handler
        // ============================================================================
        public string DecryptString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Decode base64url or base64
            byte[] bytes;
            try { bytes = WebEncoders.Base64UrlDecode(value); }
            catch
            {
                try { bytes = Convert.FromBase64String(value); }
                catch { return null; }
            }

            string decoded = Encoding.UTF8.GetString(bytes);

            // V2 prefix?
            if (decoded.StartsWith(PrefixV2, StringComparison.Ordinal))
            {
                string payload = decoded.Substring(PrefixV2.Length);
                return modern.DecryptString(payload);
            }

            // Otherwise → legacy v1
            return legacy.DecryptString(decoded);
        }
    }
}
