using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chat.Utility
{
    public class TextBlockBuilder
    {
        private Brush  _color   = new SolidColorBrush(Colors.White);
        private string _message = string.Empty;

        /// <summary>
        /// Set text color
        /// </summary>
        /// <param name="color">Color to set</param>
        /// <returns>self for chaining</returns>
        public TextBlockBuilder WithColor(Color color)
        {
            _color = new SolidColorBrush(color);
            return this;
        }

        /// <summary>
        /// Set message
        /// </summary>
        /// <param name="message">Message to set</param>
        /// <returns>self for chaining</returns>
        public TextBlockBuilder WithMessage(string message)
        {
            _message = message;
            return this;
        }

        /// <summary>
        /// Build the text block
        /// </summary>
        /// <returns>The built text block</returns>
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