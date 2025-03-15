using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
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
        private readonly IInspectionRepository _inspectionRepository;

        [ObservableProperty]
        private ObservableCollection<Appointment> appointments;

        [ObservableProperty]
        private Appointment selectedAppointment;

        [ObservableProperty]
        private DateTime? selectedDate;

        [ObservableProperty]
        private string selectedStatus;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string debugInfo;

        public bool HasAppointments => Appointments != null && Appointments.Count > 0;
        public bool HasNoAppointments => !HasAppointments;

        public StationViewModel(IStationService stationService)
        {
            _stationService = stationService;
            _inspectionRepository = ((App)Application.Current)._serviceProvider.GetService<IInspectionRepository>();

            // Khởi tạo các giá trị mặc định
            SelectedDate = DateTime.Today;
            SelectedStatus = "All";
            Appointments = new ObservableCollection<Appointment>();
            DebugInfo = "Chưa có thông tin debug";

            Debug.WriteLine($"StationViewModel khởi tạo với ngày: {SelectedDate}, trạng thái: {SelectedStatus}");
        }

        partial void OnSelectedDateChanged(DateTime? value)
        {
            Debug.WriteLine($"Ngày thay đổi thành: {value:dd/MM/yyyy}");

            // Tránh trigger nhiều lần khi property thay đổi
            if (!IsLoading)
            {
                LoadAppointmentsCommand.ExecuteAsync(null).ConfigureAwait(false);
            }
        }

        partial void OnSelectedStatusChanged(string value)
        {
            Debug.WriteLine($"Trạng thái thay đổi thành: {value}");

            // Tránh trigger nhiều lần khi property thay đổi
            if (!IsLoading)
            {
                LoadAppointmentsCommand.ExecuteAsync(null).ConfigureAwait(false);
            }
        }

        [RelayCommand]
        private async Task LoadAppointments()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                Debug.WriteLine("Bắt đầu tải lịch hẹn");

                // Chuẩn bị thông tin debug
                var debugBuilder = new StringBuilder();

                // In UserID và role để xác nhận đúng tài khoản đang đăng nhập
                int stationId = AuthService.CurrentUser.UserID;
                debugBuilder.AppendLine($"Người dùng hiện tại: {AuthService.CurrentUser.FullName}, Vai trò: {AuthService.CurrentUser.Role}");
                debugBuilder.AppendLine($"ID người dùng hiện tại (StationID): {stationId}");
                debugBuilder.AppendLine($"Ngày đã chọn: {SelectedDate:dd/MM/yyyy}, Trạng thái: {SelectedStatus}");
                Debug.WriteLine(debugBuilder.ToString());

                // Tải lịch hẹn với ngày mặc định
                var result = await _stationService.GetAppointmentsAsync(stationId, SelectedDate, SelectedStatus);
                debugBuilder.AppendLine($"Số lịch hẹn tìm thấy: {result?.Count ?? 0}");
                Debug.WriteLine($"Số lịch hẹn tìm thấy: {result?.Count ?? 0}");

                // Nếu không tìm thấy kết quả, thử với các ngày khác
                if (result == null || result.Count == 0)
                {
                    debugBuilder.AppendLine("Không tìm thấy lịch hẹn nào với ngày đã chọn, thử các ngày khác...");

                    // Thử các ngày cụ thể trong CSDL
                    var datesToTry = new List<DateTime>
                    {
                        new DateTime(2025, 3, 11),
                        new DateTime(2025, 3, 16),
                        new DateTime(2025, 3, 1)
                    };

                    foreach (var dateToTry in datesToTry)
                    {
                        debugBuilder.AppendLine($"Thử với ngày: {dateToTry:dd/MM/yyyy}");
                        var altResult = await _stationService.GetAppointmentsAsync(stationId, dateToTry, SelectedStatus);
                        debugBuilder.AppendLine($"  --> Tìm thấy {altResult?.Count ?? 0} lịch hẹn");

                        if (altResult != null && altResult.Count > 0)
                        {
                            result = altResult;
                            SelectedDate = dateToTry; // Cập nhật ngày đã chọn
                            debugBuilder.AppendLine($"Đã tìm thấy lịch hẹn cho ngày {dateToTry:dd/MM/yyyy}!");
                            break;
                        }
                    }

                    // Nếu vẫn không tìm thấy, thử tải tất cả
                    if ((result == null || result.Count == 0) && SelectedDate.HasValue)
                    {
                        debugBuilder.AppendLine("Thử tải tất cả lịch hẹn (không lọc theo ngày)...");
                        result = await _stationService.GetAppointmentsAsync(stationId, null, SelectedStatus);
                        debugBuilder.AppendLine($"  --> Tìm thấy {result?.Count ?? 0} lịch hẹn");
                    }
                }

                // Clear và thêm từng item
                Appointments.Clear();
                if (result != null && result.Count > 0)
                {
                    foreach (var appointment in result)
                    {
                        Appointments.Add(appointment);
                        debugBuilder.AppendLine($"Đã thêm: ID {appointment.AppointmentID}, Xe: {appointment.Vehicle?.PlateNumber}, Ngày: {appointment.AppointmentDate:dd/MM/yyyy HH:mm}");
                        Debug.WriteLine($"Đã thêm lịch hẹn: {appointment.AppointmentID}, Xe: {appointment.Vehicle?.PlateNumber}, Ngày: {appointment.AppointmentDate:dd/MM/yyyy HH:mm}");
                    }
                }
                else
                {
                    Debug.WriteLine("Không tìm thấy lịch hẹn nào");
                    debugBuilder.AppendLine("Không tìm thấy lịch hẹn nào");
                }

                // Cập nhật thông tin debug
                DebugInfo = debugBuilder.ToString();

                // Thông báo cho UI biết HasAppointments đã thay đổi
                OnPropertyChanged(nameof(HasAppointments));
                OnPropertyChanged(nameof(HasNoAppointments));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi khi tải lịch hẹn: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                DebugInfo = $"Lỗi: {ex.Message}\n{ex.StackTrace}";

                MessageBox.Show($"Lỗi khi tải danh sách lịch hẹn: {ex.Message}",
                              "Lỗi",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ConfirmAppointment(Appointment appointment)
        {
            try
            {
                if (appointment == null) return;
                Debug.WriteLine($"Đang xác nhận lịch hẹn: {appointment.AppointmentID}");

                IsLoading = true;
                var result = await _stationService.ConfirmAppointmentAsync(appointment.AppointmentID, AuthService.CurrentUser.UserID);

                if (result)
                {
                    MessageBox.Show("Xác nhận lịch hẹn thành công!",
                                  "Thành công",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);

                    // Cập nhật UI
                    appointment.Status = "Confirmed";
                    int index = Appointments.IndexOf(appointment);
                    if (index >= 0)
                    {
                        Appointments[index] = appointment;
                    }

                    // Tải lại dữ liệu để đảm bảo đồng bộ
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
                Debug.WriteLine($"Lỗi khi xác nhận lịch hẹn: {ex.Message}");
                MessageBox.Show($"Lỗi khi xác nhận lịch hẹn: {ex.Message}",
                              "Lỗi",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task RejectAppointment(Appointment appointment)
        {
            try
            {
                if (appointment == null) return;
                Debug.WriteLine($"Đang từ chối lịch hẹn: {appointment.AppointmentID}");

                var dialog = new RejectReasonDialog();
                if (dialog.ShowDialog() != true) return;

                string reason = dialog.Reason;
                Debug.WriteLine($"Lý do từ chối: {reason}");

                IsLoading = true;
                var result = await _stationService.RejectAppointmentAsync(appointment.AppointmentID, reason);

                if (result)
                {
                    MessageBox.Show("Từ chối lịch hẹn thành công!",
                                  "Thành công",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);

                    // Cập nhật UI
                    appointment.Status = "Cancelled";
                    int index = Appointments.IndexOf(appointment);
                    if (index >= 0)
                    {
                        Appointments[index] = appointment;
                    }

                    // Tải lại dữ liệu để đảm bảo đồng bộ
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
                Debug.WriteLine($"Lỗi khi từ chối lịch hẹn: {ex.Message}");
                MessageBox.Show($"Lỗi khi từ chối lịch hẹn: {ex.Message}",
                              "Lỗi",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task StartInspection(Appointment appointment)
        {
            try
            {
                if (appointment == null) return;
                Debug.WriteLine($"Bắt đầu kiểm định cho lịch hẹn: {appointment.AppointmentID}");

                var inspectionDialog = new InspectionInputDialog(appointment);
                var result = inspectionDialog.ShowDialog();

                if (result == true)
                {
                    // Dialog đã xử lý việc lưu kết quả và cập nhật trạng thái appointment
                    // Tải lại dữ liệu để cập nhật UI
                    await LoadAppointmentsCommand.ExecuteAsync(null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi khi bắt đầu kiểm định: {ex.Message}");
                MessageBox.Show($"Lỗi khi bắt đầu kiểm định: {ex.Message}",
                              "Lỗi",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
    }
}