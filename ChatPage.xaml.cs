using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using StoryForge.Models;
using StoryForge.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;

namespace StoryForge.Pages
{
    public sealed partial class ChatPage : Page
    {
        private readonly ObservableCollection<ChatMessage> _chatMessages = [];
        private readonly LlmService _llmService;
        //private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public ChatPage()
        {
            InitializeComponent();

            _llmService = new LlmService();
            ChatMessagesList.ItemsSource = _chatMessages;

            // Check if we have any existing messages
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

            // Auto-scroll based on setting
            bool autoScroll = true; /*!(_localSettings.Values.TryGetValue("AutoScrollToNewMessages", out var autoScrollObj) && 
                autoScrollObj is bool autoScrollValue && !autoScrollValue);*/
                
            if (autoScroll)
            {
                ChatScrollViewer.ChangeView(null, double.MaxValue, null);
            }

            // Show thinking indicator based on setting
            bool showThinking = true; /* !(_localSettings.Values.TryGetValue("ShowThinkingIndicator", out var showThinkingObj) &&
                showThinkingObj is bool showThinkingValue && !showThinkingValue); */
                
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
}