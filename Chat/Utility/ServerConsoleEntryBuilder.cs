using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chat.Utility
{
    public class ServerConsoleEntryBuilder
    {
        private string _message = string.Empty;
        private Brush  _color   = new SolidColorBrush(Colors.White);
        
        public ServerConsoleEntryBuilder WithColor(Color color)
        {
            _color = new SolidColorBrush(color);
            return this;
        }

        public ServerConsoleEntryBuilder WithMessage(string message)
        {
            _message = message;
            return this;
        }

        public TextBlock Build()
        {
            TextBlock textBlock = new TextBlock
            {
                Text       = _message,
                Foreground = _color,
                Margin = new Thickness(5, 1, 1, 5),
            };

            return textBlock;
        }
    }
}