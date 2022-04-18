using System.Windows;
using Chat.Utility;

namespace Chat.Windows
{
    public partial class Start
    {
        public Start() { InitializeComponent(); }

        private void BtnJoinServer_OnClick(object sender, RoutedEventArgs e) { WindowUtility.OpenNewWindowAndCloseCurrentOne<LoginWindow>(this); }

        private void BtnHostServer_OnClick(object sender, RoutedEventArgs e) { WindowUtility.OpenNewWindowAndCloseCurrentOne<ServerWindow>(this); }
    }
}