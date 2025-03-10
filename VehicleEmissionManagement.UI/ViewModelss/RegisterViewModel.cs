using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.UI.Viewss;

namespace VehicleEmissionManagement.UI.ViewModelss
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string fullName;
        [ObservableProperty]
        private string email;
        [ObservableProperty]
        private string password;
        [ObservableProperty]
        private string confirmPassword;
        [ObservableProperty]
        private string phone;
        [ObservableProperty]
        private string address;

        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Email) ||
                string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
            {
                MessageBox.Show("Please fill all required fields");
                return;
            }

            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Passwords do not match");
                return;
            }

            var user = new User
            {
                FullName = FullName,
                Email = Email,
                Password = Password,
                Phone = Phone,
                Address = Address,
                Role = "Owner",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var result = await _authService.RegisterAsync(user);
            if (result)
            {
                MessageBox.Show("Registration successful!");
                var loginWindow = new LoginView();
                loginWindow.Show();
                Application.Current.Windows.OfType<RegisterView>().FirstOrDefault()?.Close();
            }
            else
            {
                MessageBox.Show("Registration failed. Please try again.");
            }
        }

        [RelayCommand]
        private void BackToLogin()
        {
            var loginWindow = new LoginView();
            loginWindow.Show();
            Application.Current.Windows.OfType<RegisterView>().FirstOrDefault()?.Close();
        }
    }
}
