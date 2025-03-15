using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VehicleEmissionManagement.Core.Servicess;
using VehicleEmissionManagement.UI.ViewModelss;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class AppointmentManagementView2 : UserControl
    {
        public AppointmentManagementView2()
        {
            InitializeComponent();
            Debug.WriteLine("AppointmentManagementView2 constructor được gọi");

            // Đăng ký event để load dữ liệu khi control được rendered
            this.Loaded += AppointmentManagementView_Loaded;
        }

        private async void AppointmentManagementView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Debug.WriteLine("AppointmentManagementView2 Loaded event được kích hoạt");

            if (DataContext is StationViewModel viewModel)
            {
                try
                {
                    // Kiểm tra thông tin người dùng hiện tại
                    if (AuthService.CurrentUser != null)
                    {
                        Debug.WriteLine($"User hiện tại: {AuthService.CurrentUser.FullName}");
                        Debug.WriteLine($"UserID: {AuthService.CurrentUser.UserID}, Role: {AuthService.CurrentUser.Role}");

                        // Hiển thị thông báo cho người dùng để debug
                        MessageBox.Show($"Đang đăng nhập với tài khoản: {AuthService.CurrentUser.FullName}\nUserID: {AuthService.CurrentUser.UserID}\nRole: {AuthService.CurrentUser.Role}",
                            "Thông tin người dùng", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Debug.WriteLine("AuthService.CurrentUser là null");
                        MessageBox.Show("Không thể lấy thông tin người dùng hiện tại", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    // Thử với nhiều ngày khác nhau
                    await TryMultipleDates(viewModel);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Lỗi trong AppointmentManagementView_Loaded: {ex.Message}");
                    Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                Debug.WriteLine($"DataContext không phải StationViewModel, nó là: {DataContext?.GetType().Name ?? "null"}");
                MessageBox.Show("Lỗi: DataContext không phải StationViewModel", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task TryMultipleDates(StationViewModel viewModel)
        {
            // Các ngày cần thử
            var datesToTry = new List<DateTime>
            {
                new DateTime(2025, 3, 16),    // Ngày trong DB
                new DateTime(2025, 3, 11),    // Ngày trong DB
                new DateTime(2025, 3, 1),     // Ngày trong DB
                DateTime.Today,               // Hôm nay
                DateTime.Today.AddDays(1)     // Ngày mai
            };

            // Cấu hình mặc định
            viewModel.SelectedStatus = "All";     // Không lọc theo trạng thái

            // Thử lần lượt các ngày
            foreach (var date in datesToTry)
            {
                Debug.WriteLine($"Đang thử tải lịch hẹn cho ngày: {date:dd/MM/yyyy}");
                viewModel.SelectedDate = date;
                await viewModel.LoadAppointmentsCommand.ExecuteAsync(null);

                // Nếu đã tìm thấy dữ liệu thì dừng
                if (viewModel.Appointments.Count > 0)
                {
                    Debug.WriteLine($"Đã tìm thấy {viewModel.Appointments.Count} lịch hẹn cho ngày {date:dd/MM/yyyy}");

                    // Thông báo cho người dùng
                    MessageBox.Show($"Đã tìm thấy {viewModel.Appointments.Count} lịch hẹn cho ngày {date:dd/MM/yyyy}",
                        "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Ngắt 100ms để tránh treo UI
                await Task.Delay(100);
            }

            // Thử tải tất cả (không lọc theo ngày)
            Debug.WriteLine("Thử tải tất cả lịch hẹn (không lọc theo ngày)");
            viewModel.SelectedDate = null;
            await viewModel.LoadAppointmentsCommand.ExecuteAsync(null);

            // Thông báo kết quả
            if (viewModel.Appointments.Count > 0)
            {
                Debug.WriteLine($"Đã tìm thấy {viewModel.Appointments.Count} lịch hẹn khi không lọc theo ngày");
                MessageBox.Show($"Đã tìm thấy {viewModel.Appointments.Count} lịch hẹn khi không lọc theo ngày",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                Debug.WriteLine("Không tìm thấy lịch hẹn nào trong cơ sở dữ liệu");
                MessageBox.Show("Không tìm thấy lịch hẹn nào trong cơ sở dữ liệu. Có thể có vấn đề với kết nối DB hoặc StationID không khớp.",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}