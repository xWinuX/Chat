using System.Windows;
using Chat.Utility;

namespace Chat.Windows
{
    public partial class Login
    {
        public Login() { InitializeComponent(); }

        private void BtnLogin_OnClick(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(TxbPort.Text, out int port)) { WindowUtility.OpenNewWindowAndCloseCurrentOne(new Client(TxbUserName.Text, TxbAddress.Text, port), this); }
            else { WindowUtility.ShowErrorMessageBox(this, "Please enter a valid port"); }
        }
    }
}