using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace StoryForge.Services;

public class SettingsService
{
    private readonly string _settingsFilePath;
    private readonly Settings _currentSettings;

    public static readonly string[] AvailableModels =
    [
        "gpt-4",
        "gpt-3.5-turbo",
        "claude-3-opus",
        "claude-3-sonnet",
        "llama3"
    ];

    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };

    private static SettingsService? _instance;

    public static SettingsService Instance
    {
        get
        {
            if (_instance != null) return _instance;

            _instance = new SettingsService();
            return _instance;
        }
    }

    private SettingsService()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(localAppData, "StoryForge");

        Directory.CreateDirectory(appFolder);

        _settingsFilePath = Path.Combine(appFolder, "settings.json");
        _currentSettings = new Settings();

        if (!File.Exists(_settingsFilePath)) return;

        try
        {
            var json = File.ReadAllText(_settingsFilePath);
            var settings = JsonSerializer.Deserialize<Settings>(json);
            if (settings != null)
            {
                _currentSettings = settings;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
        }
    }

    public void SaveSettingsToFile()
    {
        try
        {
            var json = JsonSerializer.Serialize(_currentSettings, JsonSerializerOptions);
            File.WriteAllText(_settingsFilePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
        }
    }

    public string GetApiEndpoint()
    {
        return _currentSettings.ApiEndpoint;
    }

    public void SetApiEndpoint(string endpoint)
    {
        _currentSettings.ApiEndpoint = endpoint;
    }

    public string GetApiKey()
    {
        return _currentSettings.ApiKey;
    }

    public void SetApiKey(string key)
    {
        _currentSettings.ApiKey = key;
    }

    public string GetSelectedModel()
    {
        return _currentSettings.SelectedModel;
    }

    public int GetSelectedModelIndex()
    {
        var model = GetSelectedModel();
        var index = Array.IndexOf(AvailableModels, model);
        return index >= 0 ? index : 0;
    }

    public void SetSelectedModel(int index)
    {
        if (index < 0 || index >= AvailableModels.Length) return;
        _currentSettings.SelectedModel = AvailableModels[index];
    }

    public bool GetAutoScrollToNewMessages()
    {
        return _currentSettings.AutoScrollToNewMessages;
    }

    public void SetAutoScrollToNewMessages(bool autoScroll)
    {
        _currentSettings.AutoScrollToNewMessages = autoScroll;
    }

    public bool GetShowThinkingIndicator()
    {
        return _currentSettings.ShowThinkingIndicator;
    }

    public void SetShowThinkingIndicator(bool showIndicator)
    {
        _currentSettings.ShowThinkingIndicator = showIndicator;
    }

    private class Settings
    {
        public string ApiEndpoint { get; set; } = "https://api.example.com/v1/chat";
        public string ApiKey { get; set; } = string.Empty;
        public string SelectedModel { get; set; } = "gpt-4";
        public bool AutoScrollToNewMessages { get; set; } = true;
        public bool ShowThinkingIndicator { get; set; } = true;
    }
}