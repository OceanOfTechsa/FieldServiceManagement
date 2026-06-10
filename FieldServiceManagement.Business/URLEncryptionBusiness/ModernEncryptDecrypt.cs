using ElmahCore;
using FieldServiceManagement.Business.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;

namespace FieldServiceManagement.Business.URLEncryptionBusiness
{
    public class ModernEncryptDecrypt : IEncryptDecrypt
    {
        private static readonly byte[] salt;

        static ModernEncryptDecrypt()
        {
            var val = AppSettings.GetUrLEncryptionKey();
            if (string.IsNullOrWhiteSpace(val))
                val = "http://www.fms.oceanoftechsa.com/";
            salt = Encoding.ASCII.GetBytes(val);
        }

        private readonly string sharedSecret;
        private const string PrefixV2 = "v2|";

        public ModernEncryptDecrypt(string sharedSecret)
        {
            this.sharedSecret = sharedSecret;
        }

        public string EncryptString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            Aes aes = Aes.Create();
            try
            {
                aes.Key = Rfc2898DeriveBytes.Pbkdf2(
                    password: Encoding.UTF8.GetBytes(sharedSecret),
                    salt: salt,
                    iterations: 10000,
                    hashAlgorithm: HashAlgorithmName.SHA256,
                    outputLength: aes.KeySize / 8
                );

                using var ms = new MemoryStream();
                ms.Write(BitConverter.GetBytes(aes.IV.Length));
                ms.Write(aes.IV);

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(value);
                }

                string base64 = Convert.ToBase64String(ms.ToArray());
                string tagged = PrefixV2 + base64;

                return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(tagged));
            }
            catch (Exception ex)
            {
                ElmahExtensions.RaiseError(new Exception("Error encrypting", ex));
                return null;
            }
            finally
            {
                aes?.Clear();
            }
        }

        public string DecryptString(string base64)
        {
            try
            {
                byte[] data = Convert.FromBase64String(base64);

                using var ms = new MemoryStream(data);
                using var aes = Aes.Create();

                aes.Key = Rfc2898DeriveBytes.Pbkdf2(
                    password: Encoding.UTF8.GetBytes(sharedSecret),
                    salt: salt,
                    iterations: 10000,
                    hashAlgorithm: HashAlgorithmName.SHA256,
                    outputLength: aes.KeySize / 8
                );

                aes.IV = ReadIV(ms);

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

        public static byte[] ReadIV(Stream s)
        {
            try
            {
                byte[] rawLength = new byte[sizeof(int)];

                // MUST use offset + count overload
                s.ReadExactly(rawLength);

                int length = BitConverter.ToInt32(rawLength, 0);
                byte[] buffer = new byte[length];

                s.ReadExactly(buffer);

                return buffer;
            }
            catch (Exception ex)
            {
                ElmahExtensions.RaiseError(new Exception("Error reading IV", ex));
                return Array.Empty<byte>();
            }
        }


    }
}
