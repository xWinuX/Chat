using System.Windows;

namespace Chat.Utility
{
    public static class WindowUtility
    {
        /// <summary>
        /// Opens the window given in the generic and closes the the one given as a parameter
        /// </summary>
        /// <param name="current">Current window to close</param>
        /// <typeparam name="TNewWindow">New window to create</typeparam>
        public static void OpenNewWindowAndCloseCurrentOne<TNewWindow>(Window current) where TNewWindow : Window, new()
        {
            TNewWindow window = new TNewWindow();
            window.Activate();
            current.Close();
            window.ShowDialog();
        }

        /// <summary>
        /// Opens the given window and closes the other given one
        /// </summary>
        /// <param name="newWindow">New window to open</param>
        /// <param name="current">Current window to close</param>
        public static void OpenNewWindowAndCloseCurrentOne(Window newWindow, Window current)
        {
            newWindow.Activate();
            current.Close();
            newWindow.ShowDialog();
        }

        /// <summary>
        /// Opens a error message box
        /// </summary>
        /// <param name="owner">Message box owner</param>
        /// <param name="text">Text to display inside the messagebox</param>
        public static void ShowErrorMessageBox(Window owner, string text) { MessageBox.Show(owner, text, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
}