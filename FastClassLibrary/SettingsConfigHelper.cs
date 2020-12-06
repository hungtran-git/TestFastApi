using Microsoft.Extensions.Configuration;
using System.IO;

namespace FastClassLibrary
{
    public class SettingsConfigHelper
    {
        private static SettingsConfigHelper _appSettings;

        public AppConfig appConfig { get; set; }

        public static AppConfig AppSetting(string Key)
        {
            _appSettings = GetCurrentSettings(Key);
            return _appSettings.appConfig;
        }

        public SettingsConfigHelper(IConfiguration config, string Key)
        {
            //this.appSettingValue = config.GetValue<string>(Key); 
            this.appConfig = config.Get<AppConfig>();
        }

        // Get a valued stored in the appsettings.
        // Pass in a key like TestArea:TestKey to get TestValue
        public static SettingsConfigHelper GetCurrentSettings(string Key)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();

            var settings = new SettingsConfigHelper(configuration.GetSection("ApplicationSettings"), Key);

            return settings;
        }
    }
}
