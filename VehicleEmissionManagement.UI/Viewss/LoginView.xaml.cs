using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using VehicleEmissionManagement.UI.ViewModelss;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy email từ TextBox
            string email = EmailTextBox.Text;

            // Kiểm tra email có định dạng @gmail.com không
            if (!IsValidGmailAddress(email))
            {
                // Hiển thị thông báo lỗi
                EmailErrorTextBlock.Visibility = Visibility.Visible;
                // Ngăn không cho login
                e.Handled = true;
            }
            else
            {
                // Ẩn thông báo lỗi nếu email hợp lệ
                EmailErrorTextBlock.Visibility = Visibility.Collapsed;

                // Không ngăn chặn sự kiện, cho phép Command trong ViewModel xử lý
                // Command sẽ được kích hoạt sau khi xử lý sự kiện này
            }
        }

        // Hàm kiểm tra email có phải định dạng @gmail.com không
        private bool IsValidGmailAddress(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Kiểm tra email có định dạng hợp lệ và kết thúc bằng @gmail.com
            string pattern = @"^[a-zA-Z0-9._%+-]+@gmail\.com$";
            return Regex.IsMatch(email, pattern);
        }
    }
}