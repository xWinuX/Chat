using System.Windows;

namespace Chat
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ChangeLabel.Content = "Thaksdjkasd";
        }
    }
}