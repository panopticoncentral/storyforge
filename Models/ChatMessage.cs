using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;

namespace StoryForge
{
    public class ChatMessage
    {
        public string SenderName { get; set; } = string.Empty;
        public string MessageText { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsFromUser { get; set; }

        public HorizontalAlignment MessageAlignment => IsFromUser ? HorizontalAlignment.Right : HorizontalAlignment.Left;

        public SolidColorBrush SenderColor => IsFromUser
            ? (SolidColorBrush)Application.Current.Resources["SystemAccentColor"]
            : new SolidColorBrush(Microsoft.UI.Colors.Gray);

        public SolidColorBrush MessageBackground => IsFromUser
            ? new SolidColorBrush(Microsoft.UI.Colors.LightBlue)
            : new SolidColorBrush(Microsoft.UI.Colors.LightGray);

        public ChatMessage(string sender, string message, bool isFromUser)
        {
            SenderName = sender;
            MessageText = message;
            IsFromUser = isFromUser;
        }
    }
}