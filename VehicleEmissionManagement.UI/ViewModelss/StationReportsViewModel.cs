using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Core.Servicess;

namespace VehicleEmissionManagement.UI.ViewModelss
{
    public partial class StationReportsViewModel : ObservableObject
    {
        private readonly IStationService _stationService;

        [ObservableProperty]
        private DateTime startDate;

        [ObservableProperty]
        private DateTime endDate;

        [ObservableProperty]
        private ObservableCollection<InspectionRecord> inspectionRecords;

        [ObservableProperty]
        private int totalInspections;

        [ObservableProperty]
        private int passCount;

        [ObservableProperty]
        private int failCount;

        [ObservableProperty]
        private double passRate;

        public StationReportsViewModel(IStationService stationService)
        {
            _stationService = stationService;

            // Mặc định là báo cáo trong tháng hiện tại
            StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            EndDate = DateTime.Today;

            InspectionRecords = new ObservableCollection<InspectionRecord>();
            LoadReportData();
        }

        [RelayCommand]
        private async Task LoadReportData()
        {
            try
            {
                int stationId = AuthService.CurrentUser.UserID;

                // Lấy thống kê
                var statistics = await _stationService.GetStatisticsAsync(stationId, StartDate, EndDate);

                TotalInspections = statistics["TotalInspections"];
                PassCount = statistics["PassCount"];
                FailCount = statistics["FailCount"];

                PassRate = TotalInspections > 0 ? (double)PassCount / TotalInspections * 100 : 0;

                // Lấy danh sách kiểm định
                var inspections = await _stationService.GetInspectionHistoryAsync(stationId, StartDate, EndDate);
                InspectionRecords = new ObservableCollection<InspectionRecord>(inspections);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu báo cáo: {ex.Message}",
                              "Lỗi",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        partial void OnStartDateChanged(DateTime value)
        {
            if (StartDate > EndDate)
            {
                EndDate = StartDate.AddDays(30);
            }
            LoadReportDataCommand.ExecuteAsync(null);
        }

        partial void OnEndDateChanged(DateTime value)
        {
            if (EndDate < StartDate)
            {
                StartDate = EndDate.AddDays(-30);
            }
            LoadReportDataCommand.ExecuteAsync(null);
        }
    }
}