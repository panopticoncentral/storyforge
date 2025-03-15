using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using System;

namespace StoryForge
{
    public sealed partial class MainWindow
    {
        public static MainWindow? CurrentWindow { get; private set; }

        private readonly Type _chatPageType = typeof(ChatPage);
        private readonly Type _settingsPageType = typeof(SettingsPage);

        public MainWindow()
        {
            InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;

            CurrentWindow = this;

            ContentFrame.Navigate(_chatPageType);
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer == null || args.SelectedItemContainer.Tag is not string navItemTag) return;
            HeaderText.Text = navItemTag;
            NavigateToPage(navItemTag);
        }

        private void NavigateToPage(string navItemTag)
        {
            var pageType = navItemTag switch
            {
                "Chat" => _chatPageType,
                "Settings" => _settingsPageType,
                _ => null
            };

            if (pageType != null)
            {
                ContentFrame.Navigate(pageType);
            }
        }
    }
}