using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Servicess;
using VehicleEmissionManagement.UI.ViewModelss;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class StationDashboard : Window
    {
        private readonly IStationService _stationService;
        private readonly IAuthService _authService;
        private readonly INotificationRepository _notificationRepository;

        // Các view được tải lazily
        private AppointmentManagementView _appointmentView;
        private StationReportsView _reportsView;

        public StationDashboard()
        {
            InitializeComponent();

            // Lấy các service từ DI
            _authService = ((App)Application.Current)._serviceProvider.GetService<IAuthService>();
            _stationService = ((App)Application.Current)._serviceProvider.GetService<IStationService>();
            _notificationRepository = ((App)Application.Current)._serviceProvider.GetService<INotificationRepository>();

            Title = $"Trạm Đăng kiểm - {AuthService.CurrentUser.FullName}";
        }

        private void HideAllPanels()
        {
            WelcomePanel.Visibility = Visibility.Collapsed;
            AppointmentPanel.Visibility = Visibility.Collapsed;
            ReportsPanel.Visibility = Visibility.Collapsed;
        }

        private void AppointmentManagement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideAllPanels();

                // Mỗi lần click sẽ tạo mới view để tránh vấn đề context
                _appointmentView = new AppointmentManagementView
                {
                    DataContext = new StationViewModel(_stationService)
                };

                AppointmentPanel.Children.Clear();
                AppointmentPanel.Children.Add(_appointmentView);
                AppointmentPanel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở quản lý lịch hẹn: {ex.Message}",
                              "Lỗi",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideAllPanels();

                // Mỗi lần click sẽ tạo mới view để tránh vấn đề context
                _reportsView = new StationReportsView
                {
                    DataContext = new StationReportsViewModel(_stationService)
                };

                ReportsPanel.Children.Clear();
                ReportsPanel.Children.Add(_reportsView);
                ReportsPanel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở báo cáo: {ex.Message}",
                              "Lỗi",
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
                MessageBox.Show($"Lỗi khi mở thông báo: {ex.Message}",
                              "Lỗi",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void StationProfile_Click(object sender, RoutedEventArgs e)
        {
            // Hiển thị thông tin trạm và cho phép chỉnh sửa
            MessageBox.Show("Chức năng Thông tin trạm đang được phát triển",
                          "Thông báo",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
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
    }
}