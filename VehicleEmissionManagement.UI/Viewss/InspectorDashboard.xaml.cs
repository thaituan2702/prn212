using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Servicess;
using VehicleEmissionManagement.UI.ViewModelss;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class InspectorDashboard : Window
    {
        private readonly IInspectorService _inspectorService;
        private readonly IAuthService _authService;
        private readonly InspectorDashboardViewModel _viewModel;

        public InspectorDashboard()
        {
            InitializeComponent();

            // Lấy các services từ DI container
            _inspectorService = ((App)Application.Current)._serviceProvider.GetService<IInspectorService>();
            _authService = ((App)Application.Current)._serviceProvider.GetService<IAuthService>();
            var inspectionRepository = ((App)Application.Current)._serviceProvider.GetService<IInspectionRepository>();

            // Nếu service chưa được đăng ký, tạo mới
            if (_inspectorService == null)
            {
                var vehicleRepository = ((App)Application.Current)._serviceProvider.GetService<IVehicleRepository>();
                var appointmentRepository = ((App)Application.Current)._serviceProvider.GetService<IAppointmentRepository>();
                _inspectorService = new InspectorService(inspectionRepository, vehicleRepository, appointmentRepository);
            }

            // Tạo và gán ViewModel
            _viewModel = new InspectorDashboardViewModel(_inspectorService, inspectionRepository);
            DataContext = _viewModel;
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

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            DashboardPanel.Visibility = Visibility.Visible;
            // Ẩn các panel khác nếu có
        }

        private void TodayInspectionsButton_Click(object sender, RoutedEventArgs e)
        {
            // Mở view xem danh sách các lịch hẹn hôm nay
            // Hoặc chuyển đến tab tương ứng
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra xem có xe được chọn không
                if (_viewModel.SelectedAppointment != null && _viewModel.SelectedAppointment.Vehicle != null)
                {
                    var historyView = new InspectionHistory(_viewModel.SelectedAppointment.Vehicle);
                    historyView.Owner = this;
                    historyView.ShowDialog();
                }
                else
                {
                    // Có thể hiển thị cửa sổ lịch sử tổng hợp hoặc thông báo
                    MessageBox.Show("Please select a vehicle to view its inspection history.",
                                  "Information",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening history view: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
        private void PendingInspectionsButton_Click(object sender, RoutedEventArgs e)
        {
            // Mở view xem danh sách các buổi kiểm định đang chờ
            // Hoặc chuyển đến tab tương ứng
        }
    }
}