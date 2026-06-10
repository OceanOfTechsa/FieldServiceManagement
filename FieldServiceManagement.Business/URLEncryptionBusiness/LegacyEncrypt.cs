using ElmahCore;
using FieldServiceManagement.Business.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace FieldServiceManagement.Business.URLEncryptionBusiness
{
    /// <summary>
    /// Legacy encryption/decryption (PBKDF2 iterations = 3)
    /// This is used ONLY for pre-v2 URLs.
    /// </summary>
    public class LegacyEncryptDecrypt : IEncryptDecrypt
    {
        private static readonly byte[] salt;

        static LegacyEncryptDecrypt()
        {
            var val = AppSettings.GetUrLEncryptionKey();
            if (string.IsNullOrWhiteSpace(val))
                val = "http://www.dutappfactory.tech/";

            salt = Encoding.ASCII.GetBytes(val);
        }

        private readonly string sharedSecret;

        public LegacyEncryptDecrypt(string sharedSecret)
        {
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException(nameof(sharedSecret));

            this.sharedSecret = sharedSecret;
        }

        // =====================================================================
        // DECRYPT STRING (Legacy PBKDF2 iteration=3)
        // =====================================================================
        public string DecryptString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // Step 1 — Base64Url or Base64 decode
            byte[] bytes;
            try { bytes = WebEncoders.Base64UrlDecode(value); }
            catch
            {
                try { bytes = Convert.FromBase64String(value); }
                catch { return null; }
            }

            // Step 2 — unwrap ASCII(Base64)
            string inner = Encoding.ASCII.GetString(bytes);
            if (inner != value && inner.IsBase64String())
                value = inner;

            if (!value.IsBase64String())
                return null;

            // Now decrypt using PBKDF2(3)
            return TryDecrypt(value, sharedSecret, salt, 3);
        }

        // =====================================================================
        // PBKDF2 DECRYPT HELPER
        // =====================================================================
        private string TryDecrypt(string base64, string secret, byte[] salt, int iterations)
        {
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(base64);

                using var ms = new MemoryStream(encryptedBytes);
                using var aes = Aes.Create();

                aes.Key = Rfc2898DeriveBytes.Pbkdf2(
                    password: Encoding.UTF8.GetBytes(secret),
                    salt: salt,
                    iterations: iterations,
                    hashAlgorithm: HashAlgorithmName.SHA256,
                    outputLength: aes.KeySize / 8
                );

                aes.IV = ReadByteArray(ms);

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);

                return sr.ReadToEnd();
            }
            catch
            {
                return null;
            }
        }

        // =====================================================================
        // READ IV LENGTH + IV BYTES
        // =====================================================================
        private static byte[] ReadByteArray(Stream s)
        {
            try
            {
                byte[] rawLength = new byte[sizeof(int)];
                if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
                    throw new SystemException("Invalid byte array length");

                int length = BitConverter.ToInt32(rawLength, 0);
                byte[] buffer = new byte[length];

                if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
                    throw new SystemException("Byte array incomplete");

                return buffer;
            }
            catch (Exception ex)
            {
                ElmahExtensions.RaiseError(new Exception("Error reading byte array", ex));
                return Array.Empty<byte>();
            }
        }

        // =====================================================================
        // LEGACY ENCRYPTION (rarely needed)
        // =====================================================================
        public string EncryptString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            string output = null;
            Aes aesAlg = null;

            try
            {
                aesAlg = Aes.Create();

                aesAlg.Key = Rfc2898DeriveBytes.Pbkdf2(
                    password: Encoding.UTF8.GetBytes(sharedSecret),
                    salt: salt,
                    iterations: 3,                        // LEGACY iteration count
                    hashAlgorithm: HashAlgorithmName.SHA256,
                    outputLength: aesAlg.KeySize / 8
                );

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using var ms = new MemoryStream();
                ms.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                ms.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(value);
                }

                output = Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                ElmahExtensions.RaiseError(new Exception("Error encrypting (legacy)", ex));
            }
            finally
            {
                aesAlg?.Clear();
            }

            return WebEncoders.Base64UrlEncode(Encoding.ASCII.GetBytes(output ?? ""));
        }
    }
}
