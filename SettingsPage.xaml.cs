using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;

namespace StoryForge.Pages
{
    public sealed partial class SettingsPage : Page
    {
        // Settings properties
        public bool IsDarkTheme { get; set; }
        public string ApiEndpoint { get; set; } = "";
        public string ApiKey { get; set; } = "";
        public int SelectedModelIndex { get; set; }
        public int MaxResponseLength { get; set; } = 2000;
        public bool AutoScrollToNewMessages { get; set; } = true;
        public bool ShowThinkingIndicator { get; set; } = true;

        //private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public SettingsPage()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Load theme setting
            IsDarkTheme = false /*_localSettings.Values.TryGetValue("IsDarkTheme", out var darkTheme) &&
                darkTheme is bool darkThemeValue && darkThemeValue*/;

            // Load API settings
            ApiEndpoint = /*_localSettings.Values.TryGetValue("ApiEndpoint", out var endpoint) &&
                endpoint is string endpointValue ? endpointValue :*/ "https://api.example.com/v1/chat";

            ApiKey = /*_localSettings.Values.TryGetValue("ApiKey", out var key) &&
                key is string keyValue ? keyValue :*/ "";

            SelectedModelIndex = /*_localSettings.Values.TryGetValue("SelectedModelIndex", out var modelIndex) &&
                modelIndex is int modelIndexValue ? modelIndexValue :*/ 0;

            // Load chat settings
            MaxResponseLength = /*_localSettings.Values.TryGetValue("MaxResponseLength", out var maxLength) &&
                maxLength is int maxLengthValue ? maxLengthValue :*/ 2000;

            AutoScrollToNewMessages = true /*!(_localSettings.Values.TryGetValue("AutoScrollToNewMessages", out var autoScroll) &&
                autoScroll is bool autoScrollValue && !autoScrollValue)*/;

            ShowThinkingIndicator = true /*!(_localSettings.Values.TryGetValue("ShowThinkingIndicator", out var showThinking) &&
                showThinking is bool showThinkingValue && !showThinkingValue)*/;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            ShowSettingsSavedNotification();
        }

        private void SaveSettings()
        {
            // Save theme setting
            //_localSettings.Values["IsDarkTheme"] = IsDarkTheme;

            // Save API settings
            //_localSettings.Values["ApiEndpoint"] = ApiEndpoint;
            //_localSettings.Values["ApiKey"] = ApiKey;
            //_localSettings.Values["SelectedModelIndex"] = SelectedModelIndex;

            // Save chat settings
            //_localSettings.Values["MaxResponseLength"] = MaxResponseLength;
            //_localSettings.Values["AutoScrollToNewMessages"] = AutoScrollToNewMessages;
            //_localSettings.Values["ShowThinkingIndicator"] = ShowThinkingIndicator;
        }

        private async void ShowSettingsSavedNotification()
        {
            var dialog = new ContentDialog
            {
                Title = "Settings Saved",
                Content = "Your settings have been saved. Some changes may require restarting the application.",
                CloseButtonText = "OK",
                XamlRoot = Content.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Ask if the user wants to save changes before leaving
            bool hasChanges = CheckForUnsavedChanges();

            if (hasChanges)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Save Changes?",
                    Content = "Do you want to save your changes before going back?",
                    PrimaryButtonText = "Save",
                    SecondaryButtonText = "Don't Save",
                    CloseButtonText = "Cancel",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = Content.XamlRoot
                };

                ContentDialogResult result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    // Save settings
                    SaveSettings();
                    ShowSettingsSavedNotification();
                }
                else if (result == ContentDialogResult.Secondary)
                {
                    // Don't save, just go back
                }
                else
                {
                    // User canceled, stay on settings page
                    return;
                }
            }

            // Navigate back
            if (MainWindow.CurrentWindow != null)
            {
                MainWindow.CurrentWindow.NavigateBackFromSettings();
            }
        }

        private bool CheckForUnsavedChanges()
        {
            // This is a simplified check - in a real app, you would compare current values with saved values
            // For now, we'll assume changes might have been made
            return true;
        }
    }
}