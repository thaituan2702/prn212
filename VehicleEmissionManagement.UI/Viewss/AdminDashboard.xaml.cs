using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.UI.ViewModelss;
using Microsoft.Extensions.DependencyInjection;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class AdminDashboard : Window
    {
        public AdminDashboard()
        {
            InitializeComponent(); // Bật lại dòng này, không throw Exception
            var userRepository = ((App)Application.Current)._serviceProvider.GetService<IUserRepository>();
            DataContext = new AdminViewModel(userRepository);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var authService = ((App)Application.Current)._serviceProvider.GetService<IAuthService>();
            authService.Logout();
            var loginWindow = new LoginView
            {
                DataContext = ((App)Application.Current)._serviceProvider.GetRequiredService<LoginViewModel>()
            };
            loginWindow.Show();
            Close();
        }
    }
}