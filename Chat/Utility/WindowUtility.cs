using System.Windows;

namespace Chat.Utility
{
    public static class WindowUtility
    {
        public static void OpenNewWindowAndCloseCurrentOne<TNewWindow>(Window current) where TNewWindow : Window, new()
        {
            TNewWindow window = new TNewWindow();
            window.Activate();
            current.Close();
            window.ShowDialog();
        }
    }
}