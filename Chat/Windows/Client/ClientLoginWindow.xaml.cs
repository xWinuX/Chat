using System.Windows;
using Chat.Utility;

namespace Chat.Windows.Client
{
    public partial class ClientLoginWindow
    {
        public ClientLoginWindow() { InitializeComponent(); }

        private void BtnLogin_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ApfAddressPort.Validate()) { return; }

            // Validate Username not empty
            if (string.IsNullOrWhiteSpace(TxbUserName.Text))
            {
                WindowUtility.ShowErrorMessageBox(this, "Username can't be empty");
                return;
            }

            // Open client window
            WindowUtility.OpenNewWindowAndCloseCurrentOne(new ClientWindow(TxbUserName.Text, ApfAddressPort.Address, ApfAddressPort.Port), this);
        }
    }
}