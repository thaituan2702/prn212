using System.Windows;
using System.Windows.Controls;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class UserDialog : Window
    {
        private readonly User _user;

        public UserDialog(User user)
        {
            InitializeComponent();
            _user = user;
            DataContext = _user;

            if (!string.IsNullOrEmpty(_user.Password))
            {
                passwordBox.Password = _user.Password;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                _user.Password = passwordBox.Password;
                _user.UpdatedAt = DateTime.Now;
                if (_user.CreatedAt == default)
                {
                    _user.CreatedAt = DateTime.Now;
                }

                // Xử lý giá trị Role trước khi lưu
                if (_user.Role.StartsWith("System.Windows.Controls.ComboBoxItem: "))
                {
                    _user.Role = _user.Role.Replace("System.Windows.Controls.ComboBoxItem: ", "");
                }

                DialogResult = true;
                Close();
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(_user.FullName) ||
                string.IsNullOrWhiteSpace(_user.Email) ||
                string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                MessageBox.Show("Please fill all required fields");
                return false;
            }
            return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}