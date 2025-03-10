using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.UI.ViewModelss;
using VehicleEmissionManagement.UI.ViewModelss;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class PoliceDashboard : Window
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IInspectionRepository _inspectionRepository;
        private readonly IAuthService _authService;

        public PoliceDashboard(IVehicleRepository vehicleRepository, IInspectionRepository inspectionRepository)
        {
            InitializeComponent();
            _vehicleRepository = vehicleRepository;
            _inspectionRepository = inspectionRepository;
            _authService = ((App)Application.Current)._serviceProvider.GetService<IAuthService>();

            DataContext = new PoliceViewModel(_vehicleRepository, _inspectionRepository);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _authService.Logout();
            var loginWindow = new LoginView
            {
                DataContext = ((App)Application.Current)._serviceProvider.GetRequiredService<LoginViewModel>()
            };
            loginWindow.Show();
            Close();
        }

        private void SearchVehicle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideAllPanels();
                SearchPanel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing search panel: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void Violations_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideAllPanels();
                ViolationsPanel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing violations panel: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reportsWindow = new ReportsWindow();
                reportsWindow.Owner = this;
                reportsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening reports: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void Notifications_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var notificationWindow = new NotificationWindow();
                notificationWindow.Owner = this;
                notificationWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening notifications: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void HideAllPanels()
        {
            WelcomePanel.Visibility = Visibility.Collapsed;
            SearchPanel.Visibility = Visibility.Collapsed;
            ViolationsPanel.Visibility = Visibility.Collapsed;
        }
    }
}