using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using StoryForge.Models;
using StoryForge.Services;

namespace StoryForge;

public sealed partial class ChatPage
{
    private readonly ObservableCollection<ChatMessage> _chatMessages = [];
    private readonly LlmService _llmService;
    private readonly SettingsService _settingsService;

    public ChatPage()
    {
        InitializeComponent();

        _settingsService = SettingsService.Instance;
        _llmService = new LlmService();
        ChatMessagesList.ItemsSource = _chatMessages;

        if (_chatMessages.Count == 0)
        {
            _chatMessages.Add(new ChatMessage("StoryForge", "Hello! I'm your AI assistant. How can I help you today?", false));
        }
    }

    private async void SendButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await SendMessage();
        }
        catch (Exception)
        {
            // Ignore it
        }
    }

    private async void MessageInputBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        try
        {
            if (e is { Key: Windows.System.VirtualKey.Enter, KeyStatus: { IsMenuKeyDown: false, IsExtendedKey: false } })
            {
                e.Handled = true;
                await SendMessage();
            }
        }
        catch (Exception)
        {
            // Ignore it
        }
    }

    private async Task SendMessage()
    {
        var userMessage = MessageInputBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(userMessage))
            return;

        _chatMessages.Add(new ChatMessage("You", userMessage, true));

        MessageInputBox.Text = string.Empty;

        var autoScroll = _settingsService.GetAutoScrollToNewMessages();

        if (autoScroll)
        {
            ChatScrollViewer.ChangeView(null, double.MaxValue, null);
        }

        // Show thinking indicator based on setting
        var showThinking = _settingsService.GetShowThinkingIndicator();

        ChatMessage? thinkingMessage = null;

        if (showThinking)
        {
            thinkingMessage = new ChatMessage("StoryForge", "Thinking...", false);
            _chatMessages.Add(thinkingMessage);
        }

        try
        {
            var response = await _llmService.GetResponseAsync(userMessage);

            if (thinkingMessage != null)
            {
                _chatMessages.Remove(thinkingMessage);
            }

            _chatMessages.Add(new ChatMessage("StoryForge", response, false));
        }
        catch (Exception ex)
        {
            if (thinkingMessage != null)
            {
                _chatMessages.Remove(thinkingMessage);
            }

            _chatMessages.Add(new ChatMessage("System", $"Error: {ex.Message}", false));
        }

        if (autoScroll)
        {
            ChatScrollViewer.ChangeView(null, double.MaxValue, null);
        }
    }
}