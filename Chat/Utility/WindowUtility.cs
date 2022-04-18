﻿using System.Text;
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

        public static void OpenNewWindowAndCloseCurrentOne(Window newWindow, Window current)
        {
            newWindow.Activate();
            current.Close();
            newWindow.ShowDialog();
        }

        public static void ShowErrorMessageBox(Window owner, string text) { MessageBox.Show(owner, text, "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
}