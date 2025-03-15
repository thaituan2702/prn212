using System.Windows;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class RejectReasonDialog : Window
    {
        public string Reason { get; private set; }

        public RejectReasonDialog()
        {
            InitializeComponent();
            ReasonTextBox.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ReasonTextBox.Text))
            {
                MessageBox.Show("Vui lòng nhập lý do từ chối.",
                              "Thông báo",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return;
            }

            Reason = ReasonTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}