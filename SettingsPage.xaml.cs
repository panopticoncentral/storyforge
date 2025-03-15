namespace StoryForge;

public sealed partial class SettingsPage
{
    public string ApiEndpoint { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public int SelectedModelIndex { get; set; }

    public bool AutoScrollToNewMessages { get; set; } = true;
    public bool ShowThinkingIndicator { get; set; } = true;

    public SettingsPage()
    {
        InitializeComponent();
    }
}