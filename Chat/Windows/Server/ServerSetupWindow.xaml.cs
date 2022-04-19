using System.Windows;
using Chat.Utility;

namespace Chat.Windows.Server
{
    public partial class ServerSetupWindow
    {
        public ServerSetupWindow() { InitializeComponent(); }

        private void BtnStartServer_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ApfAddressPort.Validate()) { return; }

            WindowUtility.OpenNewWindowAndCloseCurrentOne(new ServerWindow(ApfAddressPort.Address, ApfAddressPort.Port), this);
        }
    }
}