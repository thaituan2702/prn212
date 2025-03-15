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
    public partial class InspectionHistoryViewModel : ObservableObject
    {
        private readonly IInspectorService _inspectorService;

        [ObservableProperty]
        private ObservableCollection<InspectionRecord> _inspectionRecords;

        [ObservableProperty]
        private InspectionRecord _selectedInspection;

        [ObservableProperty]
        private string _searchTerm;

        [ObservableProperty]
        private DateTime _startDate;

        [ObservableProperty]
        private DateTime _endDate;

        [ObservableProperty]
        private bool _isLoading;

        public InspectionHistoryViewModel(IInspectorService inspectorService)
        {
            _inspectorService = inspectorService;
            InspectionRecords = new ObservableCollection<InspectionRecord>();

            // Mặc định tìm kiếm 7 ngày gần đây
            StartDate = DateTime.Today.AddDays(-7);
            EndDate = DateTime.Today;

            // Load dữ liệu khi khởi tạo
            LoadInspectionsCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private async Task LoadInspections()
        {
            try
            {
                IsLoading = true;
                Debug.WriteLine($"Loading inspections from {StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}");

                int inspectorId = AuthService.CurrentUser.UserID;
                var inspections = await _inspectorService.GetCompletedInspectionsAsync(inspectorId, StartDate, EndDate);

                InspectionRecords.Clear();
                foreach (var inspection in inspections)
                {
                    InspectionRecords.Add(inspection);
                }

                Debug.WriteLine($"Loaded {InspectionRecords.Count} inspection records");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading inspections: {ex.Message}");
                MessageBox.Show($"Error loading inspections: {ex.Message}",
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
        private async Task SearchInspections()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchTerm))
                {
                    await LoadInspectionsCommand.ExecuteAsync(null);
                    return;
                }

                IsLoading = true;
                Debug.WriteLine($"Searching inspections with term: {SearchTerm}");

                int inspectorId = AuthService.CurrentUser.UserID;
                var searchResults = await _inspectorService.SearchInspectionsAsync(inspectorId, SearchTerm);

                InspectionRecords.Clear();
                foreach (var inspection in searchResults)
                {
                    InspectionRecords.Add(inspection);
                }

                Debug.WriteLine($"Found {InspectionRecords.Count} matching records");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error searching inspections: {ex.Message}");
                MessageBox.Show($"Error searching inspections: {ex.Message}",
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
        private void ViewDetails(InspectionRecord inspection)
        {
            if (inspection == null) return;

            try
            {
                var detailsWindow = new InspectionDetailsDialog(inspection);
                detailsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error viewing inspection details: {ex.Message}");
                MessageBox.Show($"Error viewing inspection details: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
    }
}