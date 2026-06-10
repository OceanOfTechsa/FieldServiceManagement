using FieldServiceManagement.Business.Configuration;

namespace FieldServiceManagement.Business.URLEncryptionBusiness
{
    public class UrlEncryptionBusiness
    {
        public static string EncryptParam(string value)
        {
            return new EncryptDecrypt(AppSettings.GetUrLEncryptionKey()).EncryptString(value);
        }

        public static string DecryptParam(string value)
        {
            return new EncryptDecrypt(AppSettings.GetUrLEncryptionKey()).DecryptString(value);
        }
    }
}
