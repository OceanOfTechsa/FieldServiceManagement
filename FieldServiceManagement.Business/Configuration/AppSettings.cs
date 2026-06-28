using System.Net.Mail;

namespace FieldServiceManagement.Business.Configuration
{
    public class AppSettings
    {
        public static readonly string CompanyName = "Ocean of Tech";
        public static readonly string CompanyUrl = "https://oceanoftech.co.za";
        public static SettingsBusiness.SettingsBusiness settingBusiness = new SettingsBusiness.SettingsBusiness();
        public static readonly string EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
        public static string baseUrl => GetConfigValue("siteUrl");
        public static readonly string AzureBlobUri = string.Empty;

        public static readonly string BaseUrl = ConfigHelper.Settings("AppSettings", "siteUrl");
        public static string InstrumentationKey => ConfigHelper.Settings("AppSettings", "InstrumentationKey") ?? string.Empty;
        public static string EmailTemplatePathFSM => Path.Combine(Environment.CurrentDirectory, @"Views/Shared/_EmailTamplateFSM.cshtml");
        public static int DailyEmailSendingLimit => Convert.ToInt32(settingBusiness.GetSettingsByKey("DailyEmailSendingLimit")?.value);
        public static string fsmLogo => "/Assets/Images/Brand/logo.svg";
        public static int staffPageSize => Convert.ToInt16(ConfigHelper.Settings("AppSettings", "staffPageSize"));
        public static string GetUrLEncryptionKey()
        {
            if (EnvironmentName != Enum.Environment.Development.ToString())
                return ConfigHelper.Settings("AppSettings", "mvcDecryptFilterSecret");
            var secret = Environment.GetEnvironmentVariable("mvcDecryptFilterSecret");
            if (string.IsNullOrEmpty(secret))
            {
                throw new Exception("Cannot read the URL encryption key !");
            }
            return secret;
        }

        //The connection string must include the Column Encryption Setting=enabled; at the end as it needs to be removed for Elmah
        public static string GetFormsConnectionString()
        {
            var connectionString = EnvironmentName != Enum.Environment.Development.ToString() ? ConfigHelper.Settings("AppSettings", "FieldServiceManagement") : Environment.GetEnvironmentVariable("FieldServiceManagement");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Cannot read the Forms Connection String !");
            }
            return connectionString;
        }
        public static string GetRedisConnectionString()
        {
            var redisConnection = EnvironmentName != Enum.Environment.Development.ToString() ? ConfigHelper.Settings("AppSettings", "FieldServiceManagementREDIS") : Environment.GetEnvironmentVariable("FieldServiceManagementREDIS");

            if (string.IsNullOrEmpty(redisConnection))
            {
                throw new Exception("Cannot read the Forms Redis connection string !");
            }
            return redisConnection;
        }

        public static string GetResendApiToken()
        {
            return ConfigHelper.Settings("AppSettings", "ResendApiToken");
        }

        public static string GetFSMFromEmail()
        {
            var from = EnvironmentName == Enum.Environment.Development.ToString()
                ? Environment.GetEnvironmentVariable("FSMFromEmail")
                : ConfigHelper.Settings("AppSettings", "FSMFromEmail");

            if (string.IsNullOrWhiteSpace(from))
                throw new InvalidOperationException("FSMFromEmail configuration is missing or empty. Set FSMFromEmail in config or environment.");

            try
            {
                var ma = new MailAddress(from);
                return string.IsNullOrWhiteSpace(ma.DisplayName) ? ma.Address : $"{ma.DisplayName} <{ma.Address}>";
            }
            catch (FormatException)
            {
                throw new InvalidOperationException($"FSMFromEmail is invalid: '{from}'. Use 'email@example.com' or 'Name <email@example.com>'.");
            }
        }




        private static string GetConfigValue(string key, string defaultValue = "")
        {
            return ConfigHelper.Settings("AppSettings", key) ?? defaultValue;
        }

    }
}
