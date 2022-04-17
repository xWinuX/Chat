using System.Windows;
using Chat.Utility;

namespace Chat.Windows
{
    public partial class Login
    {
        public Login() { InitializeComponent(); }

        private void BtnLogin_OnClick(object sender, RoutedEventArgs e)
        {
            WindowUtility.OpenNewWindowAndCloseCurrentOne(new Client(TxbUserName.Text), this);
        }
    }
}