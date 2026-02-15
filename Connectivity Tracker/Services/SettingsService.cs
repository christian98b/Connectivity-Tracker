using System.IO;
using System.Text.Json;
using Connectivity_Tracker.Models;

namespace Connectivity_Tracker.Services
{
    public class SettingsService
    {
        private readonly string _settingsPath;
        private AppSettings _currentSettings;

        public AppSettings CurrentSettings => _currentSettings;

        public SettingsService()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ConnectivityTracker"
            );

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            _settingsPath = Path.Combine(appDataPath, "settings.json");
            _currentSettings = LoadSettings();
        }

        public AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    _currentSettings = settings ?? new AppSettings();
                }
                else
                {
                    _currentSettings = new AppSettings();
                    SaveSettings(_currentSettings);
                }
            }
            catch
            {
                _currentSettings = new AppSettings();
            }

            return _currentSettings;
        }

        public void SaveSettings(AppSettings settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(_settingsPath, json);
                _currentSettings = settings;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save settings: {ex.Message}");
            }
        }

        public event EventHandler? SettingsChanged;

        public void UpdateSettings(AppSettings settings)
        {
            SaveSettings(settings);
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
