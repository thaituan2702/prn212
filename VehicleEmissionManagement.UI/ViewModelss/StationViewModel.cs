using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Core.Servicess;
using System.Linq;

namespace VehicleEmissionManagement.UI.ViewModelss
{
    public partial class StationViewModel : ObservableObject
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IInspectionRepository _inspectionRepository;
        private readonly AppointmentService _appointmentService;

        [ObservableProperty]
        private ObservableCollection<Appointment> _pendingAppointments;

        [ObservableProperty]
        private ObservableCollection<Appointment> _todayAppointments;

        [ObservableProperty]
        private ObservableCollection<InspectionRecord> _recentInspections;

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Today;

        [ObservableProperty]
        private string _selectedStatus = "Tất cả";

        [ObservableProperty]
        private Appointment _selectedAppointment;

        [ObservableProperty]
        private int _totalInspections;

        [ObservableProperty]
        private int _passInspections;

        [ObservableProperty]
        private int _failInspections;

        [ObservableProperty]
        private string _statusFilter = "All";

        // Thống kê theo tháng
        [ObservableProperty]
        private DateTime _startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        [ObservableProperty]
        private DateTime _endDate = DateTime.Now;

        // Mảng dữ liệu báo cáo theo tháng
        [ObservableProperty]
        private ObservableCollection<MonthlyStatistics> _monthlyStatistics;

        public ObservableCollection<string> StatusFilters { get; } = new ObservableCollection<string>
        {
            "All", "Pending", "Confirmed", "Completed", "Cancelled"
        };

        public StationViewModel(IAppointmentRepository appointmentRepository, IInspectionRepository inspectionRepository)
        {
            _appointmentRepository = appointmentRepository;
            _inspectionRepository = inspectionRepository;
            _appointmentService = new AppointmentService(_appointmentRepository);

            PendingAppointments = new ObservableCollection<Appointment>();
            TodayAppointments = new ObservableCollection<Appointment>();
            RecentInspections = new ObservableCollection<InspectionRecord>();
            MonthlyStatistics = new ObservableCollection<MonthlyStatistics>();

            // Không gọi LoadDashboardDataAsync ở đây để tránh lỗi
            // Thay vào đó, hãy gọi nó từ code-behind khi view đã load xong
        }

        [RelayCommand]
        private async Task LoadDashboardDataAsync()
        {
            try
            {
                // Gọi tuần tự từng phương thức async và đợi kết quả
                await LoadAppointmentsAsync();

                // Chờ một khoảng thời gian ngắn để đảm bảo hoàn tất thao tác trước
                await Task.Delay(100);

                await LoadRecentInspectionsAsync();

                await Task.Delay(100);

                await LoadStatisticsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu dashboard: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task LoadAppointmentsAsync()
        {
            try
            {
                int stationId = AuthService.CurrentUser.UserID;

                // Load các cuộc hẹn riêng biệt
                var pendingAppointmentsResult = await _appointmentRepository.GetAppointmentsByStationIdAsync(
                    stationId, null, "Pending");

                Application.Current.Dispatcher.Invoke(() => {
                    PendingAppointments.Clear();
                    foreach (var item in pendingAppointmentsResult)
                    {
                        PendingAppointments.Add(item);
                    }
                });

                await Task.Delay(50); // Thêm delay ngắn

                // Lấy các cuộc hẹn hôm nay
                var todayAppointmentsResult = await _appointmentRepository.GetAppointmentsByStationIdAsync(
                    stationId, DateTime.Today, StatusFilter == "All" ? null : StatusFilter);

                Application.Current.Dispatcher.Invoke(() => {
                    TodayAppointments.Clear();
                    foreach (var item in todayAppointmentsResult)
                    {
                        TodayAppointments.Add(item);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách lịch hẹn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task LoadRecentInspectionsAsync()
        {
            try
            {
                int stationId = AuthService.CurrentUser.UserID;
                var inspections = await _inspectionRepository.GetInspectionsByStationIdAsync(stationId);

                Application.Current.Dispatcher.Invoke(() => {
                    RecentInspections.Clear();
                    foreach (var item in inspections)
                    {
                        RecentInspections.Add(item);
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lịch sử kiểm định: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task LoadStatisticsAsync()
        {
            try
            {
                int stationId = AuthService.CurrentUser.UserID;
                var statistics = await _inspectionRepository.GetInspectionStatisticsAsync(stationId, StartDate, EndDate);

                TotalInspections = statistics.ContainsKey("Total") ? statistics["Total"] : 0;
                PassInspections = statistics.ContainsKey("Pass") ? statistics["Pass"] : 0;
                FailInspections = statistics.ContainsKey("Fail") ? statistics["Fail"] : 0;

                // Tính tỷ lệ và tạo biểu đồ
                await GenerateMonthlyStatisticsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thống kê: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task GenerateMonthlyStatisticsAsync()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => {
                    MonthlyStatistics.Clear();
                });

                // Tạo thống kê cho 6 tháng gần đây
                DateTime startMonth = DateTime.Today.AddMonths(-5);
                int stationId = AuthService.CurrentUser.UserID;

                for (int i = 0; i < 6; i++)
                {
                    DateTime month = startMonth.AddMonths(i);
                    DateTime monthStart = new DateTime(month.Year, month.Month, 1);
                    DateTime monthEnd = monthStart.AddMonths(1).AddDays(-1);

                    var stats = await _inspectionRepository.GetInspectionStatisticsAsync(stationId, monthStart, monthEnd);

                    Application.Current.Dispatcher.Invoke(() => {
                        MonthlyStatistics.Add(new MonthlyStatistics
                        {
                            Month = month.ToString("MM/yyyy"),
                            Total = stats.ContainsKey("Total") ? stats["Total"] : 0,
                            Pass = stats.ContainsKey("Pass") ? stats["Pass"] : 0,
                            Fail = stats.ContainsKey("Fail") ? stats["Fail"] : 0,
                            PassRate = stats.ContainsKey("Total") && stats["Total"] > 0
                                ? (double)stats["Pass"] / stats["Total"] * 100
                                : 0
                        });
                    });

                    await Task.Delay(50); // Thêm delay ngắn
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo thống kê tháng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task ConfirmAppointmentAsync(Appointment appointment)
        {
            try
            {
                if (appointment == null) return;

                var result = MessageBox.Show(
                    $"Xác nhận lịch hẹn cho xe {appointment.Vehicle.PlateNumber} vào lúc {appointment.AppointmentDate:dd/MM/yyyy HH:mm}?",
                    "Xác nhận lịch hẹn",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _appointmentService.ConfirmAppointmentAsync(appointment.AppointmentID, AuthService.CurrentUser.UserID);
                    await Task.Delay(100); // Thêm delay ngắn
                    await LoadAppointmentsAsync();
                    MessageBox.Show("Đã xác nhận lịch hẹn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xác nhận lịch hẹn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task CancelAppointmentAsync(Appointment appointment)
        {
            try
            {
                if (appointment == null) return;

                var result = MessageBox.Show(
                    $"Bạn có chắc muốn hủy lịch hẹn cho xe {appointment.Vehicle.PlateNumber} vào lúc {appointment.AppointmentDate:dd/MM/yyyy HH:mm}?",
                    "Hủy lịch hẹn",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    await _appointmentService.CancelAppointmentAsync(appointment.AppointmentID, AuthService.CurrentUser.UserID);
                    await Task.Delay(100); // Thêm delay ngắn
                    await LoadAppointmentsAsync();
                    MessageBox.Show("Đã hủy lịch hẹn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hủy lịch hẹn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task RescheduleAppointmentAsync(object[] parameters)
        {
            try
            {
                if (parameters.Length < 2) return;

                var appointment = parameters[0] as Appointment;
                var newDate = (DateTime)parameters[1];

                if (appointment == null) return;

                await _appointmentService.ScheduleInspectionAsync(appointment.AppointmentID, newDate, AuthService.CurrentUser.UserID);
                await Task.Delay(100); // Thêm delay ngắn
                await LoadAppointmentsAsync();
                MessageBox.Show("Đã cập nhật lịch hẹn mới!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đổi lịch hẹn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task FilterAppointmentsAsync()
        {
            await LoadAppointmentsAsync();
        }

        // UI Interaction Methods
        [RelayCommand]
        private void NavigateToAppointments()
        {
            // To be implemented in the view
        }

        [RelayCommand]
        private void NavigateToInspections()
        {
            // To be implemented in the view
        }

        [RelayCommand]
        private void NavigateToReports()
        {
            // To be implemented in the view
        }
    }

    // Class hỗ trợ hiển thị thống kê hàng tháng
    public class MonthlyStatistics
    {
        public string Month { get; set; }
        public int Total { get; set; }
        public int Pass { get; set; }
        public int Fail { get; set; }
        public double PassRate { get; set; }
    }
}