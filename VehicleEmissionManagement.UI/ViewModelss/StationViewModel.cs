using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Core.Servicess;
using VehicleEmissionManagement.UI.Viewss;

namespace VehicleEmissionManagement.UI.ViewModelss
{
    public partial class StationViewModel : ObservableObject
    {
        private readonly IStationService _stationService;
        private bool _isLoading = false;

        [ObservableProperty]
        private ObservableCollection<Appointment> appointments;

        [ObservableProperty]
        private Appointment selectedAppointment;

        [ObservableProperty]
        private DateTime selectedDate;

        [ObservableProperty]
        private string selectedStatus;

        public StationViewModel(IStationService stationService)
        {
            _stationService = stationService;
            // Đặt mặc định là ngày hôm nay
            SelectedDate = DateTime.Today;
            SelectedStatus = "All";
            Appointments = new ObservableCollection<Appointment>();

            // Không load dữ liệu ngay lập tức - sẽ được gọi từ View sau khi khởi tạo
            Debug.WriteLine($"StationViewModel created with date: {SelectedDate}, status: {SelectedStatus}");
        }

        [RelayCommand]
        private async Task LoadAppointments()
        {
            if (_isLoading) return;

            try
            {
                _isLoading = true;
                Debug.WriteLine("Begin loading appointments");

                // In UserID và role để xác nhận đúng tài khoản đang đăng nhập
                int stationId = AuthService.CurrentUser.UserID;
                Debug.WriteLine($"Current user: {AuthService.CurrentUser.FullName}, Role: {AuthService.CurrentUser.Role}");
                Debug.WriteLine($"Current user ID (Station ID): {stationId}");
                Debug.WriteLine($"Selected date: {SelectedDate}, Status: {SelectedStatus}");

                // Thử thêm lệnh chạy truy vấn SQL trực tiếp để kiểm tra
                var result = await _stationService.GetAppointmentsAsync(stationId, SelectedDate, SelectedStatus);
                Debug.WriteLine($"Appointments loaded: {result?.Count ?? 0}");

                // Clear và thêm từng item
                Appointments.Clear();
                if (result != null && result.Count > 0)
                {
                    foreach (var appointment in result)
                    {
                        Appointments.Add(appointment);
                        Debug.WriteLine($"Added appointment: {appointment.AppointmentID}, Vehicle: {appointment.Vehicle?.PlateNumber}, Date: {appointment.AppointmentDate:dd/MM/yyyy HH:mm}");
                    }
                }
                else
                {
                    Debug.WriteLine("No appointments found");
                    // Thông báo cho người dùng biết không có dữ liệu
                    MessageBox.Show($"Không có lịch hẹn nào cho ngày {SelectedDate:dd/MM/yyyy} với trạng thái {SelectedStatus}",
                                  "Thông báo",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading appointments: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Lỗi khi tải danh sách lịch hẹn: {ex.Message}",
                              "Lỗi",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
            finally
            {
                _isLoading = false;
            }
        }

        [RelayCommand]
        private async Task ConfirmAppointment(Appointment appointment)
        {
            try
            {
                if (appointment == null) return;
                Debug.WriteLine($"Confirming appointment: {appointment.AppointmentID}");

                var result = await _stationService.ConfirmAppointmentAsync(appointment.AppointmentID, AuthService.CurrentUser.UserID);
                if (result)
                {
                    MessageBox.Show("Xác nhận lịch hẹn thành công!",
                                  "Thành công",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);

                    await LoadAppointmentsCommand.ExecuteAsync(null);
                }
                else
                {
                    MessageBox.Show("Không thể xác nhận lịch hẹn. Vui lòng thử lại!",
                                  "Lỗi",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error confirming appointment: {ex.Message}");
                MessageBox.Show($"Lỗi khi xác nhận lịch hẹn: {ex.Message}",
                              "Lỗi",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task RejectAppointment(Appointment appointment)
        {
            try
            {
                if (appointment == null) return;
                Debug.WriteLine($"Rejecting appointment: {appointment.AppointmentID}");

                var dialog = new RejectReasonDialog();
                if (dialog.ShowDialog() != true) return;

                string reason = dialog.Reason;
                Debug.WriteLine($"Rejection reason: {reason}");

                var result = await _stationService.RejectAppointmentAsync(appointment.AppointmentID, reason);
                if (result)
                {
                    MessageBox.Show("Từ chối lịch hẹn thành công!",
                                  "Thành công",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);

                    await LoadAppointmentsCommand.ExecuteAsync(null);
                }
                else
                {
                    MessageBox.Show("Không thể từ chối lịch hẹn. Vui lòng thử lại!",
                                  "Lỗi",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error rejecting appointment: {ex.Message}");
                MessageBox.Show($"Lỗi khi từ chối lịch hẹn: {ex.Message}",
                              "Lỗi",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            Debug.WriteLine($"Date changed to: {value:dd/MM/yyyy}");
            // Tránh trigger nhiều lần khi property thay đổi
            if (!_isLoading)
            {
                LoadAppointmentsCommand.ExecuteAsync(null).ConfigureAwait(false);
            }
        }

        partial void OnSelectedStatusChanged(string value)
        {
            Debug.WriteLine($"Status changed to: {value}");
            // Tránh trigger nhiều lần khi property thay đổi
            if (!_isLoading)
            {
                LoadAppointmentsCommand.ExecuteAsync(null).ConfigureAwait(false);
            }
        }
    }
}