using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Core.Servicess;
using VehicleEmissionManagement.UI.Viewss;
using VehicleEmissionManagement.UI.ViewModelss;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class StationDashboard : Window
    {
        private readonly StationViewModel _viewModel;

        public StationDashboard()
        {
            InitializeComponent();

            // Lấy các repository từ DI container
            var appointmentRepository = ((App)Application.Current)._serviceProvider.GetService<IAppointmentRepository>();
            var inspectionRepository = ((App)Application.Current)._serviceProvider.GetService<IInspectionRepository>();

            // Tạo và gán ViewModel
            _viewModel = new StationViewModel(appointmentRepository, inspectionRepository);
            DataContext = _viewModel;

            // Đăng ký sự kiện Loaded để load dữ liệu sau khi UI đã hiển thị
            this.Loaded += StationDashboard_Loaded;
        }

        private async void StationDashboard_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Load dữ liệu khi form đã hiển thị
                await _viewModel.LoadDashboardDataCommand.ExecuteAsync(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi khởi tạo dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        #region Navigation
        private async void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            DashboardPanel.Visibility = Visibility.Visible;
            await _viewModel.LoadDashboardDataCommand.ExecuteAsync(null);
        }

        private async void Appointments_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            AppointmentsPanel.Visibility = Visibility.Visible;
            await _viewModel.LoadAppointmentsCommand.ExecuteAsync(null);
        }

        private async void Inspections_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            InspectionsPanel.Visibility = Visibility.Visible;
            await _viewModel.LoadRecentInspectionsCommand.ExecuteAsync(null);
        }

        private async void Reports_Click(object sender, RoutedEventArgs e)
        {
            HideAllPanels();
            ReportsPanel.Visibility = Visibility.Visible;
            await _viewModel.LoadStatisticsCommand.ExecuteAsync(null);
        }

        private void HideAllPanels()
        {
            DashboardPanel.Visibility = Visibility.Collapsed;
            AppointmentsPanel.Visibility = Visibility.Collapsed;
            InspectionsPanel.Visibility = Visibility.Collapsed;
            ReportsPanel.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Appointment Actions
        private async void RescheduleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var appointment = (Appointment)((Button)sender).DataContext;
                var dialog = new RescheduleDialog(appointment);

                if (dialog.ShowDialog() == true)
                {
                    var parameters = new object[] { appointment, dialog.SelectedDateTime };
                    await _viewModel.RescheduleAppointmentCommand.ExecuteAsync(parameters);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đổi lịch hẹn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void InputInspectionResult_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var appointment = (Appointment)((Button)sender).DataContext;
                var dialog = new InspectionInputDialog(appointment);

                if (dialog.ShowDialog() == true)
                {
                    // Refresh data after adding inspection result
                    await _viewModel.LoadDashboardDataCommand.ExecuteAsync(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nhập kết quả kiểm định: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel != null)
            {
                await _viewModel.FilterAppointmentsCommand.ExecuteAsync(null);
            }
        }
        #endregion

        #region Inspection Actions
        private void InspectionDetails_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var inspection = (InspectionRecord)((Button)sender).DataContext;
                var detailsDialog = new InspectionDetailsDialog(inspection);
                detailsDialog.Owner = this;
                detailsDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị chi tiết kiểm định: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Report Actions
        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Tính năng xuất Excel sẽ được triển khai trong phiên bản sau.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                // TODO: Implement Excel export
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show("Tính năng xuất PDF sẽ được triển khai trong phiên bản sau.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                // TODO: Implement PDF export
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}