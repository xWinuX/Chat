using System.Windows.Controls;

namespace Chat.UserControls
{
    public partial class ScrollTextDisplay : UserControl
    {
        public ScrollTextDisplay() { InitializeComponent(); }
        
        public void AddText(TextBlock textBlock)
        {
            SpnlStackPanel.Children.Add(textBlock);
            ScrvScrollViewer.ScrollToBottom();
        }
    }
}