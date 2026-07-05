using FieldServiceManagement.Business.SettingsBusiness;
using System.Net.Mail;

namespace FieldServiceManagement.Business.Configuration
{
    public class AppSettings
    {
        public static readonly string CompanyName = "Ocean of Tech";
        public static readonly string SystemDiagnosisText = "Running System Diagnosis";
        public static readonly string CompanyUrl = "https://oceanoftech.co.za";
        public static SettingsBusiness.SettingsBusiness settingBusiness = new();
        public static readonly string EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
        public static string baseUrl => GetConfigValue("siteUrl");
        public static string azureBaseUrl => EnvironmentName == nameof(Enum.Environment.Development)
         ? baseUrl
         : GetConfigValue("azureUrl");

        public static readonly string BaseUrl = GetConfigValue("siteUrl");
        public static string InstrumentationKey => GetConfigValue("InstrumentationKey") ?? string.Empty;
        public static string EmailTemplatePathFSM => Path.Combine(Environment.CurrentDirectory, @"Views/Shared/_EmailTamplateFSM.cshtml");
        public static int DailyEmailSendingLimit => Convert.ToInt32(GetSettingValue("DailyEmailSendingLimit"));
        public static string fsmLogo => "/Assets/Images/Brand/logo.svg";
        public static int staffPageSize => Convert.ToInt16(GetConfigValue("staffPageSize"));
        public static string GetUrLEncryptionKey()
        {
            if (EnvironmentName != nameof(Enum.Environment.Development))
                return GetConfigValue("mvcDecryptFilterSecret");

            var secret = Environment.GetEnvironmentVariable("mvcDecryptFilterSecret");
            if (string.IsNullOrEmpty(secret))
            {
                throw new Exception("Cannot read the URL encryption key !");
            }
            return secret;
        }

        public static string GetFSMConnectionString()
        {
            var connectionString = EnvironmentName != nameof(Enum.Environment.Development)
                ? GetConfigValue("FSMConnectionString")
                : Environment.GetEnvironmentVariable("FSMConnectionString");

            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Cannot read the FSM Connection String!");

            return connectionString;
        }

        public static string GetRedisConnectionString()
        {
            var redisConnection = EnvironmentName != nameof(Enum.Environment.Development) 
                ? GetConfigValue("FSMREDIS") 
                : Environment.GetEnvironmentVariable("FSMREDIS");

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
            var from = EnvironmentName != nameof(Enum.Environment.Development)
                ? GetConfigValue("FSMFromEmail")
                : Environment.GetEnvironmentVariable("FSMFromEmail");

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

        #region Helpers
        private static string GetSettingValue(string key, string defaultValue = "")
        {
            return settingBusiness.GetSettingsByKey(key)?.value ?? defaultValue;
        }

        private static string GetConfigValue(string key, string defaultValue = "")
        {
            return ConfigHelper.Settings("AppSettings", key) ?? defaultValue;
        }

        private static bool GetBooleanSetting(string key, bool defaultValue = false)
        {
            var value = GetSettingValue(key);

            return bool.TryParse(value, out var result)
                ? result
                : defaultValue;
        }

        private static int GetIntSetting(string key, int defaultValue = 0)
        {
            var value = GetSettingValue(key);

            return int.TryParse(value, out var result)
                ? result
                : defaultValue;
        }

        private static int GetIntConfig(string key, int defaultValue = 0)
        {
            var value = GetConfigValue(key);

            return int.TryParse(value, out var result)
                ? result
                : defaultValue;
        }

        private static DateTime GetDateSetting(string key)
        {
            var value = GetSettingValue(key);

            return DateTime.TryParse(value, out var result)
                ? result
                : DateTime.MinValue;
        }
        #endregion
    }
}
