using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Core.Servicess;
using VehicleEmissionManagement.UI.Viewss;

namespace VehicleEmissionManagement.UI.ViewModelss
{
    public partial class InspectorDashboardViewModel : ObservableObject
    {
        private readonly IInspectorService _inspectorService;
        private readonly IInspectionRepository _inspectionRepository;

        [ObservableProperty]
        private ObservableCollection<Appointment> _todayAppointments;

        [ObservableProperty]
        private ObservableCollection<InspectionRecord> _recentInspections;

        [ObservableProperty]
        private ObservableCollection<InspectionRecord> _pendingInspections;

        [ObservableProperty]
        private Appointment _selectedAppointment;

        [ObservableProperty]
        private string _inspectorName;

        [ObservableProperty]
        private int _completedToday;

        [ObservableProperty]
        private bool _isLoading;

        public InspectorDashboardViewModel(IInspectorService inspectorService, IInspectionRepository inspectionRepository)
        {
            _inspectorService = inspectorService;
            _inspectionRepository = inspectionRepository;
            TodayAppointments = new ObservableCollection<Appointment>();
            RecentInspections = new ObservableCollection<InspectionRecord>();
            PendingInspections = new ObservableCollection<InspectionRecord>();
            InspectorName = AuthService.CurrentUser?.FullName ?? "Inspector";

            // Load dữ liệu khi khởi tạo
            LoadDashboardDataCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private async Task LoadDashboardData()
        {
            try
            {
                IsLoading = true;
                Debug.WriteLine("Loading dashboard data for inspector...");

                int inspectorId = AuthService.CurrentUser.UserID;
                int stationId = AuthService.CurrentUser.UserID; // Giả định stationId là UserID của inspector

                // Lấy các lịch hẹn hôm nay
                var appointments = await _inspectorService.GetTodayAppointmentsAsync(stationId);
                TodayAppointments.Clear();
                foreach (var appointment in appointments)
                {
                    TodayAppointments.Add(appointment);
                }

                // Lấy các buổi kiểm định đã hoàn thành gần đây
                var completedInspections = await _inspectorService.GetCompletedInspectionsAsync(inspectorId, DateTime.Today.AddDays(-7), DateTime.Today);
                RecentInspections.Clear();
                foreach (var inspection in completedInspections.OrderByDescending(i => i.InspectionDate).Take(10))
                {
                    RecentInspections.Add(inspection);
                }

                // Lấy các buổi kiểm định đang chờ
                var pendingInspections = await _inspectorService.GetPendingInspectionsAsync(stationId);
                PendingInspections.Clear();
                foreach (var inspection in pendingInspections)
                {
                    PendingInspections.Add(inspection);
                }

                // Đếm số buổi kiểm định đã hoàn thành hôm nay
                var todayInspections = await _inspectorService.GetCompletedInspectionsAsync(inspectorId, DateTime.Today, DateTime.Today);
                CompletedToday = todayInspections.Count;

                Debug.WriteLine($"Dashboard data loaded: {TodayAppointments.Count} appointments, {RecentInspections.Count} recent inspections");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading dashboard data: {ex.Message}");
                MessageBox.Show($"Error loading dashboard data: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
        [RelayCommand]
        private void NewInspection(Appointment appointment)
        {
            try
            {
                if (appointment == null) return;

                // Thay đổi từ
                // var inputView = new InspectionInputView(appointment);
                // Thành
                var inputView = new InspectionInputDialog(appointment);

                if (inputView.ShowDialog() == true)
                {
                    // Reload dashboard sau khi nhập xong
                    LoadDashboardDataCommand.ExecuteAsync(null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error starting new inspection: {ex.Message}");
                MessageBox.Show($"Error starting new inspection: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ViewHistory()
        {
            try
            {
                // Lỗi: InspectionHistory1 không tồn tại
                // Lỗi: InspectionHistory cần tham số Vehicle

                // Sửa lại như sau:
                // 1. Nếu muốn xem lịch sử của vehicle cụ thể
                if (SelectedAppointment != null && SelectedAppointment.Vehicle != null)
                {
                    var historyView = new InspectionHistory(SelectedAppointment.Vehicle);
                    historyView.ShowDialog();
                }
                // 2. Hoặc nếu muốn tạo view mới để xem lịch sử tổng hợp
                else
                {
                    MessageBox.Show("Please select a vehicle to view its inspection history.",
                                  "Information",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening history view: {ex.Message}");
                MessageBox.Show($"Error opening history view: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
    }
}