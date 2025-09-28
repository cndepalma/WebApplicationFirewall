using WebApplicationFirewall.Helpers;
using Newtonsoft.Json;

public class FeatureActivatorService : IFeatureActivatorService
{
    private readonly string _settingsFilePath = Path.Combine("Data", "FeatureSettings.json");

    public Settings Settings { get; private set; }

    public FeatureActivatorService(IStorageHelper storageHelper)
    {
        Settings = LoadSettings();
    }

    public void UpdateSettings(Settings newSettings)
    {
        if (newSettings != null)
        {
            Settings.RateLimitEnabled = newSettings.RateLimitEnabled;
            Settings.BlockedIpsEnabled = newSettings.BlockedIpsEnabled;
            Settings.SqlInjectionDetectionEnabled = newSettings.SqlInjectionDetectionEnabled;
            SaveSettings();
        }
    }

    private void SaveSettings()
    {
        var json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
        Directory.CreateDirectory(Path.GetDirectoryName(_settingsFilePath)!);
        File.WriteAllText(_settingsFilePath, json);
    }

    private Settings LoadSettings()
    {
        if (File.Exists(_settingsFilePath))
        {
            var json = File.ReadAllText(_settingsFilePath);
            return JsonConvert.DeserializeObject<Settings>(json) ?? new Settings();
        }
        return new Settings();
    }
}