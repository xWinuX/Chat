using System.Windows;
using Chat.Utility;

namespace Chat.Windows
{
    public partial class LoginWindow
    {
        public LoginWindow() { InitializeComponent(); }

        private void BtnLogin_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxbUserName.Text))
            {
                WindowUtility.ShowErrorMessageBox(this, "Username can't be empty");
                return;
            }

            if (int.TryParse(TxbPort.Text, out int port)) { WindowUtility.OpenNewWindowAndCloseCurrentOne(new ClientWindow(TxbUserName.Text, TxbAddress.Text, port), this); }
            else { WindowUtility.ShowErrorMessageBox(this, "Please enter a valid port"); }
        }
    }
}