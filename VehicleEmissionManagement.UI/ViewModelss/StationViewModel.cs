using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
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
            SelectedDate = DateTime.Today;
            SelectedStatus = "All";
            Appointments = new ObservableCollection<Appointment>();

            // Không load dữ liệu ngay lập tức để tránh xung đột context
            // LoadAppointmentsCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private async Task LoadAppointments()
        {
            if (_isLoading) return;

            try
            {
                _isLoading = true;

                // Thêm delay để tránh xung đột
                await Task.Delay(100);

                int stationId = AuthService.CurrentUser.UserID;
                var result = await _stationService.GetAppointmentsAsync(stationId, SelectedDate, SelectedStatus);

                Appointments.Clear();
                foreach (var appointment in result)
                {
                    Appointments.Add(appointment);
                }
            }
            catch (Exception ex)
            {
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

                // Thêm delay để tránh xung đột 
                await Task.Delay(100);

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

                var dialog = new RejectReasonDialog();
                if (dialog.ShowDialog() != true) return;

                string reason = dialog.Reason;

                // Thêm delay để tránh xung đột
                await Task.Delay(100);

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
                MessageBox.Show($"Lỗi khi từ chối lịch hẹn: {ex.Message}",
                              "Lỗi",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            // Tránh trigger nhiều lần khi property thay đổi
            if (!_isLoading)
            {
                LoadAppointmentsCommand.ExecuteAsync(null).ConfigureAwait(false);
            }
        }

        partial void OnSelectedStatusChanged(string value)
        {
            // Tránh trigger nhiều lần khi property thay đổi
            if (!_isLoading)
            {
                LoadAppointmentsCommand.ExecuteAsync(null).ConfigureAwait(false);
            }
        }
    }
}