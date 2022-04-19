using System.Windows.Controls;

namespace Chat.UserControls
{
    public partial class ScrollTextDisplay
    {
        public ScrollTextDisplay() { InitializeComponent(); }

        public void AddText(TextBlock textBlock)
        {
            SpnlStackPanel.Children.Add(textBlock);
            ScrvScrollViewer.ScrollToBottom();
        }
    }
}