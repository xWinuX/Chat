using System.Windows;
using Chat.Utility;

namespace Chat.UserControls
{
    public partial class AddressPortForm
    {
        public string Address => TxbAddress.Text;

        public int Port
        {
            get
            {
                if (int.TryParse(TxbPort.Text, out int port))
                {
                    return port;
                }

                return -1;
            }
        }

        public AddressPortForm() { InitializeComponent(); }

        /// <summary>
        /// Validate Inputs and show messageboxes when they're not
        /// </summary>
        /// <returns>True if everything is valid, false if not</returns>
        public bool Validate()
        {
            Window currentWindow = Window.GetWindow(this);
            
            // Validate address not empty
            if (string.IsNullOrWhiteSpace(Address))
            {
                WindowUtility.ShowErrorMessageBox(currentWindow, "Address can't be empty");
                return false;
            }

            // Validate Port
            if (Port <= 0)
            {
                WindowUtility.ShowErrorMessageBox(currentWindow, "Please enter a valid port");
                return false;
            }

            return true;
        }
    }
}