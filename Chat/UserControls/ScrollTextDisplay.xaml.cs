using System.Windows.Controls;

namespace Chat.UserControls
{
    public partial class ScrollTextDisplay
    {
        public ScrollTextDisplay() { InitializeComponent(); }

        /// <summary>
        /// Add text block to scroll display
        /// </summary>
        /// <param name="textBlock">Text block to add</param>
        public void AddText(TextBlock textBlock)
        {
            SpnlStackPanel.Children.Add(textBlock);
            ScrvScrollViewer.ScrollToBottom();
        }
    }
}