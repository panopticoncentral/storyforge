using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StoryForge.Services;

namespace StoryForge;

public sealed partial class SettingsPage
{
    private readonly SettingsService _settingsService;

    // Properties bound to UI controls
    public string ApiEndpoint { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public int SelectedModelIndex { get; set; }
    public bool AutoScrollToNewMessages { get; set; } = true;
    public bool ShowThinkingIndicator { get; set; } = true;

    public SettingsPage()
    {
        InitializeComponent();

        _settingsService = SettingsService.Instance;

        // Load settings
        LoadSettings();

        // Add event handlers for saving settings when the page unloads
        Unloaded += SettingsPage_Unloaded;

        // Add handler for the test connection button if it exists
        if (TestConnectionButton != null)
        {
            TestConnectionButton.Click += TestConnectionButton_Click;
        }
    }

    private void LoadSettings()
    {
        ApiEndpoint = _settingsService.GetApiEndpoint();
        ApiKey = _settingsService.GetApiKey();
        SelectedModelIndex = _settingsService.GetSelectedModelIndex();
        AutoScrollToNewMessages = _settingsService.GetAutoScrollToNewMessages();
        ShowThinkingIndicator = _settingsService.GetShowThinkingIndicator();
    }

    private void SettingsPage_Unloaded(object sender, RoutedEventArgs e)
    {
        SaveSettings();
    }

    private async void TestConnectionButton_Click(object sender, RoutedEventArgs e)
    {
        // First save current settings
        SaveSettings();

        // Show testing message
        TestConnectionButton.IsEnabled = false;
        TestConnectionButton.Content = "Testing...";

        // Test the connection
        bool isSuccessful = await TestLlmConnection();

        // Reset button
        TestConnectionButton.IsEnabled = true;
        TestConnectionButton.Content = "Test Connection";

        // Show result
        if (isSuccessful)
        {
            ShowInfoBar("Connection Successful", "Successfully connected to the LLM API.", InfoBarSeverity.Success);
        }
        else
        {
            ShowInfoBar("Connection Failed", "Failed to connect to the LLM API. Please check your settings and try again.", InfoBarSeverity.Error);
        }
    }

    private async Task<bool> TestLlmConnection()
    {
        try
        {
            // Create a new LlmService instance with the latest settings
            var llmService = new LlmService();
            llmService.UpdateApiSettings();

            // Send a simple test message
            var response = await llmService.GetResponseAsync("Hello, this is a test message to check the connection.");

            // If we get a response and it's not an error message, consider it successful
            return !response.Contains("Error communicating with LLM service");
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void SaveSettings()
    {
        _settingsService.SetApiEndpoint(ApiEndpoint);
        _settingsService.SetApiKey(ApiKey);
        _settingsService.SetSelectedModel(SelectedModelIndex);
        _settingsService.SetAutoScrollToNewMessages(AutoScrollToNewMessages);
        _settingsService.SetShowThinkingIndicator(ShowThinkingIndicator);
        _settingsService.SaveSettingsToFile();

        // Update the LlmService with the new settings
        var llmService = new LlmService();
        llmService.UpdateApiSettings();
    }

    private void ShowInfoBar(string title, string message, InfoBarSeverity severity)
    {
        // Find or create the InfoBar
        if (SettingsInfoBar != null)
        {
            SettingsInfoBar.Title = title;
            SettingsInfoBar.Message = message;
            SettingsInfoBar.Severity = severity;
            SettingsInfoBar.IsOpen = true;

            // Auto-close after 5 seconds
            DispatcherQueue.TryEnqueue(async () =>
            {
                await Task.Delay(5000);
                SettingsInfoBar.IsOpen = false;
            });
        }
    }
}