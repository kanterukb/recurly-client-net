using Microsoft.Extensions.Configuration;

namespace Recurly.Configuration
{
    public static class SettingsManager
    {
        public static void InitializeFromConfig(IConfigurationRoot configuration)
        {
            Settings.Instance.InitializeFromConfig(configuration);
        }

        public static void Initialize(string apiKey, string subdomain, string privateKey = "", int pageSize = 200)
        {
            Settings.Instance.Initialize(apiKey, subdomain, privateKey, pageSize);
        }
    }
}