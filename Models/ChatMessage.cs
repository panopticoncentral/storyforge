using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace StoryForge.Models;

public class ChatMessage(string sender, string message, bool isFromUser)
{
    public string SenderName { get; set; } = sender;
    public string MessageText { get; set; } = message;
    public bool IsFromUser { get; set; } = isFromUser;

    public HorizontalAlignment MessageAlignment => HorizontalAlignment.Left;

    public SolidColorBrush SenderColor => IsFromUser
        ? new SolidColorBrush(Microsoft.UI.Colors.Blue)
        : new SolidColorBrush(Microsoft.UI.Colors.Gray);

    public SolidColorBrush MessageBackground => IsFromUser
        ? new SolidColorBrush(Microsoft.UI.Colors.LightBlue)
        : new SolidColorBrush(Microsoft.UI.Colors.LightGray);
}