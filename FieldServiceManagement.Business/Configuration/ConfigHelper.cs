using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace FieldServiceManagement.Business.Configuration
{
    public class ConfigHelper
    {
        private static readonly IConfigurationRoot Configuration = GetCurrentSettings();
        private static readonly Dictionary<string, string> AppSettings = new();

        public static string Settings(string parent, string key)
        {
            if (Business.Configuration.AppSettings.EnvironmentName is "Production" or "UAT")
            {
                key = "fieldServiceManagement" + key;
            }
            return AppSettings.ContainsKey(key) ? AppSettings[key] : GetSettingsFromVault(parent, key);
        }

        private static string GetSettingsFromVault(string parent, string key)
        {
            lock (AppSettings)
            {
                switch (Business.Configuration.AppSettings.EnvironmentName)
                {
                    case "Development":
                        {
                            return GetKeyFromAppSettingJson(parent, key);
                        }
                }

                if (key.Contains("siteUrl"))
                {
                    return GetKeyFromAppSettingJson(parent, key);
                }

                var secretClient = new SecretClient(new Uri(Configuration["AppSettings:VaultUri"]),
                    GetVaultCredentials());
                var val = secretClient.GetSecret(key).Value.Value;
                if (!AppSettings.ContainsKey(key))
                    AppSettings.Add(key, val);
                return val;
            }
        }

        private static string GetKeyFromAppSettingJson(string parent, string key)
        {
            var value = Configuration.GetSection(parent).GetValue<string>(key);
            if (!AppSettings.ContainsKey(key))
                AppSettings.Add(key, value);
            return value;
        }

        private static TokenCredential GetVaultCredentials()
        {
            return new DefaultAzureCredential();
        }

        private static IConfigurationRoot GetCurrentSettings()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.{Business.Configuration.AppSettings.EnvironmentName}.json", optional: false, reloadOnChange: true);
                //.AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
