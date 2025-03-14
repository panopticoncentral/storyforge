using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StoryForge.Pages;
using System;

namespace StoryForge
{
    public sealed partial class MainWindow
    {
        // Static reference to the current MainWindow instance
        public static MainWindow? CurrentWindow { get; private set; }
        // Type cache for faster page navigation
        private readonly Type _chatPageType = typeof(ChatPage);
        private readonly Type _settingsPageType = typeof(SettingsPage);

        // Track the previous page before settings
        private Type _previousPageType;

        public MainWindow()
        {
            InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;

            // Set the static reference to this window
            CurrentWindow = this;

            // Navigate to chat page by default
            _previousPageType = _chatPageType;
            ContentFrame.Navigate(_chatPageType);
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // Save the current page type before navigating to settings
            if (args.IsSettingsSelected && ContentFrame.CurrentSourcePageType != _settingsPageType)
            {
                _previousPageType = ContentFrame.CurrentSourcePageType;
                UpdateHeaderText("settings");
                ContentFrame.Navigate(_settingsPageType);
                return;
            }

            if (args.SelectedItemContainer != null && args.SelectedItemContainer.Tag is string navItemTag)
            {
                UpdateHeaderText(navItemTag);
                NavigateToPage(navItemTag);
            }
        }

        private void UpdateHeaderText(string navItemTag)
        {
            HeaderText.Text = navItemTag switch
            {
                "chat" => "Chat",
                "projects" => "Projects",
                "characters" => "Characters",
                "settings" => "Settings",
                _ => "Chat"
            };
        }

        private void NavigateToPage(string navItemTag)
        {
            Type? pageType = navItemTag switch
            {
                "chat" => _chatPageType,
                _ => null
            };

            if (pageType != null)
            {
                ContentFrame.Navigate(pageType);
            }
        }

        /// <summary>
        /// Navigate back to the previous page from settings
        /// </summary>
        public void NavigateBackFromSettings()
        {
            // Find the appropriate NavView item based on previous page type
            NavigationViewItem? navItem = null;

            if (_previousPageType == _chatPageType)
            {
                navItem = ChatNavItem;
            }

            // If we found a matching item, select it
            if (navItem != null)
            {
                NavView.SelectedItem = navItem;
            }
            else
            {
                // Default to chat page if we can't determine the previous page
                NavView.SelectedItem = ChatNavItem;
            }
        }
    }
}