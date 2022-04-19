using System.Windows;
using Chat.Utility;
using Chat.Windows.Client;
using Chat.Windows.Server;

namespace Chat.Windows
{
    public partial class Start
    {
        public Start() { InitializeComponent(); }

        private void BtnJoinServer_OnClick(object sender, RoutedEventArgs e) { WindowUtility.OpenNewWindowAndCloseCurrentOne<ClientLoginWindow>(this); }

        private void BtnHostServer_OnClick(object sender, RoutedEventArgs e) { WindowUtility.OpenNewWindowAndCloseCurrentOne<ServerSetupWindow>(this); }
    }
}