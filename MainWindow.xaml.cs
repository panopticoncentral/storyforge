using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace StoryForge
{
    public sealed partial class MainWindow : Window
    {
        private ObservableCollection<ChatMessage> _chatMessages = new ObservableCollection<ChatMessage>();
        private LlmService _llmService;

        public MainWindow()
        {
            this.InitializeComponent();

            // Initialize the LLM service
            _llmService = new LlmService();

            // Bind the messages collection to the ListView
            ChatMessagesList.ItemsSource = _chatMessages;

            // Add a welcome message
            _chatMessages.Add(new ChatMessage("StoryForge", "Hello! I'm your AI assistant. How can I help you today?", false));
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await SendMessage();
        }

        private async void MessageInputBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && !e.KeyStatus.IsMenuKeyDown && !e.KeyStatus.IsExtendedKey)
            {
                // Prevent adding a newline character
                e.Handled = true;

                // Send the message
                await SendMessage();
            }
        }

        private async Task SendMessage()
        {
            string userMessage = MessageInputBox.Text.Trim();

            // Don't process empty messages
            if (string.IsNullOrWhiteSpace(userMessage))
                return;

            // Add the user message to the chat
            _chatMessages.Add(new ChatMessage("You", userMessage, true));

            // Clear the input box
            MessageInputBox.Text = string.Empty;

            // Scroll to the bottom
            ChatScrollViewer.ChangeView(null, double.MaxValue, null);

            // Add a "thinking" message
            var thinkingMessage = new ChatMessage("StoryForge", "Thinking...", false);
            _chatMessages.Add(thinkingMessage);

            try
            {
                // Get response from the LLM
                string response = await _llmService.GetResponseAsync(userMessage);

                // Remove the "thinking" message
                _chatMessages.Remove(thinkingMessage);

                // Add the LLM response
                _chatMessages.Add(new ChatMessage("StoryForge", response, false));
            }
            catch (Exception ex)
            {
                // Remove the "thinking" message
                _chatMessages.Remove(thinkingMessage);

                // Show error message
                _chatMessages.Add(new ChatMessage("System", $"Error: {ex.Message}", false));
            }

            // Scroll to the bottom again after new messages
            ChatScrollViewer.ChangeView(null, double.MaxValue, null);
        }
    }
}