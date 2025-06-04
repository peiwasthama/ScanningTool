using System;
using System.IO;
using System.Text.Json;

namespace scanningTool.Helpers
{
    /// <summary>
    /// Helper class for managing application configuration.
    /// </summary>
    public static class ConfigurationHelper
    {
        private static readonly string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appSettings.json");
        private static AppSettings _cachedSettings = null;

        /// <summary>
        /// Gets the application settings from the configuration file.
        /// </summary>
        /// <returns>The application settings.</returns>
        public static AppSettings GetAppSettings()
        {
            if (_cachedSettings != null)
                return _cachedSettings;

            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    _cachedSettings = JsonSerializer.Deserialize<AppSettings>(json);
                    return _cachedSettings;
                }
                else
                {
                    // Create default settings if file doesn't exist
                    _cachedSettings = new AppSettings
                    {
                        DumpItPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DumpIt.exe"),
                        OutputFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output"),
                        MaxEvents = 50,
                        OpenAIApiKey = ""
                    };

                    // Save default settings
                    SaveAppSettings(_cachedSettings);
                    return _cachedSettings;
                }
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error reading configuration");
                return new AppSettings
                {
                    DumpItPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DumpIt.exe"),
                    OutputFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output"),
                    MaxEvents = 50,
                    OpenAIApiKey = ""
                };
            }
        }

        /// <summary>
        /// Saves the application settings to the configuration file.
        /// </summary>
        /// <param name="settings">The settings to save.</param>
        public static void SaveAppSettings(AppSettings settings)
        {
            try
            {
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ConfigFilePath, json);
                _cachedSettings = settings;
                LoggingHelper.LogInfo("Configuration saved successfully");
            }
            catch (Exception ex)
            {
                LoggingHelper.LogException(ex, "Error saving configuration");
            }
        }

        /// <summary>
        /// Gets the path to the DumpIt executable.
        /// </summary>
        /// <returns>The path to the DumpIt executable.</returns>
        public static string GetDumpItPath()
        {
            return GetAppSettings().DumpItPath;
        }

        /// <summary>
        /// Gets the path to the output folder.
        /// </summary>
        /// <returns>The path to the output folder.</returns>
        public static string GetOutputFolderPath()
        {
            var path = GetAppSettings().OutputFolderPath;
            
            // Create the directory if it doesn't exist
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    LoggingHelper.LogException(ex, "Error creating output directory");
                }
            }
            
            return path;
        }

        /// <summary>
        /// Gets the maximum number of events to retrieve.
        /// </summary>
        /// <returns>The maximum number of events.</returns>
        public static int GetMaxEvents()
        {
            return GetAppSettings().MaxEvents;
        }

        /// <summary>
        /// Gets the OpenAI API key from configuration.
        /// </summary>
        /// <returns>The OpenAI API key.</returns>
        public static string GetOpenAIApiKey()
        {
            return GetAppSettings().OpenAIApiKey;
        }

        /// <summary>
        /// Sets the OpenAI API key in configuration.
        /// </summary>
        /// <param name="apiKey">The API key to set.</param>
        public static void SetOpenAIApiKey(string apiKey)
        {
            var settings = GetAppSettings();
            settings.OpenAIApiKey = apiKey;
            SaveAppSettings(settings);
        }
    }

    /// <summary>
    /// Class representing application settings.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the path to the DumpIt executable.
        /// </summary>
        public string DumpItPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the output folder.
        /// </summary>
        public string OutputFolderPath { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of events to retrieve.
        /// </summary>
        public int MaxEvents { get; set; }

        /// <summary>
        /// Gets or sets the OpenAI API key.
        /// </summary>
        public string OpenAIApiKey { get; set; }
    }
}
