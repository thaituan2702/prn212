using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private Dictionary<string, decimal> emissionAverages;

        [ObservableProperty]
        private ObservableCollection<ChartData> monthlyStatistics;

        [ObservableProperty]
        private ObservableCollection<ChartData> resultBreakdown;

        public bool HasInspectionRecords => InspectionRecords != null && InspectionRecords.Count > 0;
        public bool HasNoInspectionRecords => !HasInspectionRecords;

        public class ChartData
        {
            public string Label { get; set; }
            public int Value { get; set; }
            public string Color { get; set; }
            public double Percentage { get; set; }
        }

        public StationReportsViewModel(IStationService stationService)
        {
            _stationService = stationService;

            // Mặc định là báo cáo trong tháng hiện tại
            StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            EndDate = DateTime.Today;

            InspectionRecords = new ObservableCollection<InspectionRecord>();
            EmissionAverages = new Dictionary<string, decimal>();
            MonthlyStatistics = new ObservableCollection<ChartData>();
            ResultBreakdown = new ObservableCollection<ChartData>();

            LoadReportData();
        }

        [RelayCommand]
        private async Task LoadReportData()
        {
            try
            {
                IsLoading = true;
                Debug.WriteLine($"Loading report data from {StartDate:dd/MM/yyyy} to {EndDate:dd/MM/yyyy}");

                int stationId = AuthService.CurrentUser.UserID;

                // Lấy thống kê
                var statistics = await _stationService.GetStatisticsAsync(stationId, StartDate, EndDate);

                if (statistics.ContainsKey("TotalInspections"))
                {
                    TotalInspections = statistics["TotalInspections"];
                    PassCount = statistics["PassCount"];
                    FailCount = statistics["FailCount"];

                    PassRate = TotalInspections > 0 ? (double)PassCount / TotalInspections * 100 : 0;

                    // Cập nhật dữ liệu biểu đồ kết quả
                    UpdateResultBreakdown();
                }
                else
                {
                    // Không có dữ liệu hoặc key không tồn tại
                    Debug.WriteLine("No statistics found or keys don't match expected format");
                    TotalInspections = 0;
                    PassCount = 0;
                    FailCount = 0;
                    PassRate = 0;
                }

                // Lấy danh sách kiểm định
                var inspections = await _stationService.GetInspectionHistoryAsync(stationId, StartDate, EndDate);
                InspectionRecords = new ObservableCollection<InspectionRecord>(inspections);

                // Cập nhật UI properties
                OnPropertyChanged(nameof(HasInspectionRecords));
                OnPropertyChanged(nameof(HasNoInspectionRecords));

                // Tính toán các chỉ số CO2, HC, NOx trung bình
                CalculateEmissionAverages();

                // Tạo dữ liệu thống kê theo tháng
                GenerateMonthlyStatistics();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading report data: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                MessageBox.Show($"Lỗi khi tải dữ liệu báo cáo: {ex.Message}",
                              "Lỗi",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void CalculateEmissionAverages()
        {
            try
            {
                if (InspectionRecords.Count == 0)
                {
                    EmissionAverages = new Dictionary<string, decimal>
                    {
                        { "CO2", 0 },
                        { "HC", 0 },
                        { "NOx", 0 }
                    };
                    return;
                }

                decimal co2Average = InspectionRecords.Average(r => r.CO2Emission);
                decimal hcAverage = InspectionRecords.Average(r => r.HCEmission);
                decimal noxAverage = InspectionRecords.Average(r => r.NOxEmission);

                EmissionAverages = new Dictionary<string, decimal>
                {
                    { "CO2", Math.Round(co2Average, 2) },
                    { "HC", Math.Round(hcAverage, 2) },
                    { "NOx", Math.Round(noxAverage, 2) }
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error calculating emission averages: {ex.Message}");
            }
        }

        private void UpdateResultBreakdown()
        {
            ResultBreakdown.Clear();
            if (TotalInspections > 0)
            {
                ResultBreakdown.Add(new ChartData
                {
                    Label = "Đạt",
                    Value = PassCount,
                    Color = "#2ecc71",
                    Percentage = Math.Round((double)PassCount / TotalInspections * 100, 1)
                });

                ResultBreakdown.Add(new ChartData
                {
                    Label = "Không đạt",
                    Value = FailCount,
                    Color = "#e74c3c",
                    Percentage = Math.Round((double)FailCount / TotalInspections * 100, 1)
                });
            }
        }

        private void GenerateMonthlyStatistics()
        {
            try
            {
                MonthlyStatistics.Clear();

                // Nhóm theo tháng và năm
                var groupedByMonth = InspectionRecords
                    .GroupBy(r => new { r.InspectionDate.Year, r.InspectionDate.Month })
                    .Select(g => new
                    {
                        YearMonth = $"{g.Key.Year}/{g.Key.Month:00}",
                        Count = g.Count(),
                        PassCount = g.Count(r => r.Result == "Pass"),
                        FailCount = g.Count(r => r.Result == "Fail")
                    })
                    .OrderBy(g => g.YearMonth)
                    .ToList();

                // Chuyển đổi thành dữ liệu biểu đồ
                foreach (var item in groupedByMonth)
                {
                    MonthlyStatistics.Add(new ChartData
                    {
                        Label = item.YearMonth,
                        Value = item.Count,
                        Color = "#3498db",
                        Percentage = item.Count > 0 ? Math.Round((double)item.PassCount / item.Count * 100, 1) : 0
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error generating monthly statistics: {ex.Message}");
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

        [RelayCommand]
        private void ExportReport()
        {
            try
            {
                // Tạo nội dung báo cáo
                var reportContent = new StringBuilder();
                reportContent.AppendLine($"BÁO CÁO THỐNG KÊ KIỂM ĐỊNH");
                reportContent.AppendLine($"Trạm: {AuthService.CurrentUser.FullName}");
                reportContent.AppendLine($"Kỳ báo cáo: {StartDate:dd/MM/yyyy} - {EndDate:dd/MM/yyyy}");
                reportContent.AppendLine();
                reportContent.AppendLine($"Tổng số xe kiểm định: {TotalInspections}");
                reportContent.AppendLine($"Số xe đạt: {PassCount} ({PassRate:F1}%)");
                reportContent.AppendLine($"Số xe không đạt: {FailCount} ({100 - PassRate:F1}%)");
                reportContent.AppendLine();
                reportContent.AppendLine("CHỈ SỐ KHÍ THẢI TRUNG BÌNH:");
                reportContent.AppendLine($"CO2: {EmissionAverages["CO2"]} g/km");
                reportContent.AppendLine($"HC: {EmissionAverages["HC"]} ppm");
                reportContent.AppendLine($"NOx: {EmissionAverages["NOx"]} ppm");
                reportContent.AppendLine();
                reportContent.AppendLine("DANH SÁCH XE KIỂM ĐỊNH:");
                reportContent.AppendLine("STT\tBiển số\tNgày kiểm định\tKết quả\tCO2\tHC\tNOx");

                int i = 1;
                foreach (var record in InspectionRecords)
                {
                    reportContent.AppendLine($"{i}\t{record.Vehicle?.PlateNumber}\t{record.InspectionDate:dd/MM/yyyy}\t{record.Result}\t{record.CO2Emission}\t{record.HCEmission}\t{record.NOxEmission}");
                    i++;
                }

                // Hiển thị dialog để chọn vị trí lưu file
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    FileName = $"BaoCaoKiemDinh_{StartDate:ddMMyyyy}_{EndDate:ddMMyyyy}.txt"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    // Lưu file
                    System.IO.File.WriteAllText(saveFileDialog.FileName, reportContent.ToString());
                    MessageBox.Show($"Đã xuất báo cáo thành công vào file:\n{saveFileDialog.FileName}",
                                  "Thành công",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error exporting report: {ex.Message}");
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}",
                              "Lỗi",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void PrintReport()
        {
            try
            {
                // Tạo PrintDialog
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    // Tạo FlowDocument để in
                    var document = new System.Windows.Documents.FlowDocument();
                    document.PagePadding = new Thickness(50);

                    // Tiêu đề báo cáo
                    var title = new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run("BÁO CÁO THỐNG KÊ KIỂM ĐỊNH"))
                    {
                        FontSize = 18,
                        FontWeight = FontWeights.Bold,
                        TextAlignment = TextAlignment.Center
                    };
                    document.Blocks.Add(title);

                    // Thông tin chung
                    var info = new System.Windows.Documents.Paragraph();
                    info.Inlines.Add(new System.Windows.Documents.Run($"Trạm: {AuthService.CurrentUser.FullName}"));
                    info.Inlines.Add(new System.Windows.Documents.LineBreak());
                    info.Inlines.Add(new System.Windows.Documents.Run($"Kỳ báo cáo: {StartDate:dd/MM/yyyy} - {EndDate:dd/MM/yyyy}"));
                    document.Blocks.Add(info);

                    // Thông tin thống kê
                    var stats = new System.Windows.Documents.Paragraph();
                    stats.Inlines.Add(new System.Windows.Documents.Run($"Tổng số xe kiểm định: {TotalInspections}"));
                    stats.Inlines.Add(new System.Windows.Documents.LineBreak());
                    stats.Inlines.Add(new System.Windows.Documents.Run($"Số xe đạt: {PassCount} ({PassRate:F1}%)"));
                    stats.Inlines.Add(new System.Windows.Documents.LineBreak());
                    stats.Inlines.Add(new System.Windows.Documents.Run($"Số xe không đạt: {FailCount} ({100 - PassRate:F1}%)"));
                    document.Blocks.Add(stats);

                    // Chỉ số khí thải trung bình
                    var emissions = new System.Windows.Documents.Paragraph();
                    emissions.Inlines.Add(new System.Windows.Documents.Run("CHỈ SỐ KHÍ THẢI TRUNG BÌNH:") { FontWeight = FontWeights.Bold });
                    emissions.Inlines.Add(new System.Windows.Documents.LineBreak());
                    emissions.Inlines.Add(new System.Windows.Documents.Run($"CO2: {EmissionAverages["CO2"]} g/km"));
                    emissions.Inlines.Add(new System.Windows.Documents.LineBreak());
                    emissions.Inlines.Add(new System.Windows.Documents.Run($"HC: {EmissionAverages["HC"]} ppm"));
                    emissions.Inlines.Add(new System.Windows.Documents.LineBreak());
                    emissions.Inlines.Add(new System.Windows.Documents.Run($"NOx: {EmissionAverages["NOx"]} ppm"));
                    document.Blocks.Add(emissions);

                    // In báo cáo
                    printDialog.PrintDocument(((System.Windows.Documents.IDocumentPaginatorSource)document).DocumentPaginator, "Báo cáo thống kê kiểm định");

                    MessageBox.Show("Đã gửi báo cáo đến máy in!",
                                  "Thành công",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error printing report: {ex.Message}");
                MessageBox.Show($"Lỗi khi in báo cáo: {ex.Message}",
                              "Lỗi",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
    }
}