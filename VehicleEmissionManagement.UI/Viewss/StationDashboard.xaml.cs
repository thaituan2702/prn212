using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Core.Servicess;
using VehicleEmissionManagement.UI.ViewModelss;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class StationDashboard : Window
    {
        private readonly IStationService _stationService;
        private readonly IAuthService _authService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IStationRepository _stationRepository;

        // Current active panel
        private UIElement _currentPanel;

        public StationDashboard()
        {
            InitializeComponent();

            Debug.WriteLine("StationDashboard constructor được gọi");

            try
            {
                // Lấy các service từ DI
                _authService = ((App)Application.Current)._serviceProvider.GetService<IAuthService>();
                _stationService = ((App)Application.Current)._serviceProvider.GetService<IStationService>();
                _notificationRepository = ((App)Application.Current)._serviceProvider.GetService<INotificationRepository>();
                _appointmentRepository = ((App)Application.Current)._serviceProvider.GetService<IAppointmentRepository>();
                _stationRepository = ((App)Application.Current)._serviceProvider.GetService<IStationRepository>();

                // Kiểm tra và log thông tin về user hiện tại
                if (AuthService.CurrentUser != null)
                {
                    Debug.WriteLine($"StationDashboard khởi tạo cho User: {AuthService.CurrentUser.FullName}, ID: {AuthService.CurrentUser.UserID}, Role: {AuthService.CurrentUser.Role}");
                    Title = $"Trạm Đăng kiểm - {AuthService.CurrentUser.FullName}";

                    // Hiển thị thông tin người dùng để debug
                    MessageBox.Show($"Đang đăng nhập với tài khoản:\nTên: {AuthService.CurrentUser.FullName}\nUserID: {AuthService.CurrentUser.UserID}\nRole: {AuthService.CurrentUser.Role}",
                        "Thông tin đăng nhập", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    Debug.WriteLine("CẢNH BÁO: AuthService.CurrentUser là null!");
                    MessageBox.Show("Không thể lấy thông tin người dùng. Vui lòng đăng nhập lại.",
                        "Lỗi",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }

                // Thử kết nối đến CSDL và kiểm tra dữ liệu cơ bản
                Task.Run(async () => await TestDatabaseAccess());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong StationDashboard constructor: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Lỗi khởi tạo: {ex.Message}",
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async Task TestDatabaseAccess()
        {
            try
            {
                Debug.WriteLine("Đang kiểm tra kết nối cơ sở dữ liệu...");

                // Lấy StationID từ user hiện tại
                int stationId = AuthService.CurrentUser.UserID;

                // Kiểm tra số lượng lịch hẹn trong database
                var appointments = await _appointmentRepository.GetAppointmentsByStationIdAsync(stationId, null, null);

                Debug.WriteLine($"Kiểm tra cơ sở dữ liệu thành công. Tìm thấy {appointments.Count} lịch hẹn cho station {stationId}");

                // Lấy các lịch hẹn theo các ngày cụ thể
                var today = DateTime.Today;
                var dates = new[] { today, today.AddDays(1), new DateTime(2025, 3, 15), new DateTime(2025, 3, 16), new DateTime(2025, 3, 1) };

                foreach (var date in dates)
                {
                    var appsOnDate = await _appointmentRepository.GetAppointmentsByStationIdAsync(stationId, date, null);
                    Debug.WriteLine($"Lịch hẹn vào ngày {date:dd/MM/yyyy}: {appsOnDate.Count}");
                }

                // Lấy danh sách tất cả các lịch hẹn không lọc theo StationID
                var allApps = await ((IStationRepository)_stationRepository).GetAllAppointmentsAsync();
                Debug.WriteLine($"Tổng số lịch hẹn trong cơ sở dữ liệu: {allApps.Count}");

                // Kiểm tra chi tiết từng lịch hẹn
                foreach (var app in allApps)
                {
                    Debug.WriteLine($"Lịch hẹn ID: {app.AppointmentID}, StationID: {app.StationID}, Ngày: {app.AppointmentDate:dd/MM/yyyy HH:mm}, Trạng thái: {app.Status}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi kiểm tra cơ sở dữ liệu: {ex.Message}");
            }
        }

        private void ShowPanel(UIElement panel)
        {
            if (_currentPanel != null && MainContent.Children.Contains(_currentPanel))
            {
                MainContent.Children.Remove(_currentPanel);
            }

            if (!MainContent.Children.Contains(panel))
            {
                MainContent.Children.Add(panel);
            }

            _currentPanel = panel;
        }

        private async void AppointmentManagement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Debug.WriteLine("AppointmentManagement button được nhấn");

                // Tạo view mới
                var appointmentView = new AppointmentManagementView2();

                // Tạo mới StationViewModel và cấu hình
                var viewModel = new StationViewModel(_stationService);

                // Gán ViewModel cho View
                appointmentView.DataContext = viewModel;

                // Hiển thị view
                ShowPanel(appointmentView);

                Debug.WriteLine("AppointmentManagementView2 đã được hiển thị");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong AppointmentManagement_Click: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
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
                Debug.WriteLine("Reports button được nhấn");

                // Tạo view báo cáo mới
                var reportsView = new StationReportsView();

                // Cấu hình ViewModel
                reportsView.DataContext = new StationReportsViewModel(_stationService);

                // Hiển thị view
                ShowPanel(reportsView);

                Debug.WriteLine("StationReportsView đã được hiển thị");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong Reports_Click: {ex.Message}");
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
                Debug.WriteLine("Notifications button được nhấn");

                var notificationWindow = new NotificationWindow();
                notificationWindow.Owner = this;
                notificationWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong Notifications_Click: {ex.Message}");
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
            Debug.WriteLine("Logout button được nhấn");

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