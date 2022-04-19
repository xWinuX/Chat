using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chat.Utility
{
    public class TextBlockBuilder
    {
        private Brush  _color   = new SolidColorBrush(Colors.White);
        private string _message = string.Empty;

        public TextBlockBuilder WithColor(Color color)
        {
            _color = new SolidColorBrush(color);
            return this;
        }

        public TextBlockBuilder WithMessage(string message)
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
                Margin = new Thickness(5, 1, 1, 5)
            };

            return textBlock;
        }
    }
}