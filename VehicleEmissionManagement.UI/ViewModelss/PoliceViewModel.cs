using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.UI.ViewModelss
{
    public partial class PoliceViewModel : ObservableObject
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IInspectionRepository _inspectionRepository;

        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private ObservableCollection<Vehicle> vehicles;

        [ObservableProperty]
        private ObservableCollection<InspectionRecord> inspectionRecords;

        [ObservableProperty]
        private Vehicle selectedVehicle;

        public PoliceViewModel(IVehicleRepository vehicleRepository, IInspectionRepository inspectionRepository)
        {
            _vehicleRepository = vehicleRepository;
            _inspectionRepository = inspectionRepository;
            Vehicles = new ObservableCollection<Vehicle>();
            InspectionRecords = new ObservableCollection<InspectionRecord>();
        }

        [RelayCommand]
        private async Task Search()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    MessageBox.Show("Please enter a search term");
                    return;
                }

                var results = await _vehicleRepository.SearchByPlateNumber(SearchText);
                Vehicles = new ObservableCollection<Vehicle>(results);

                if (Vehicles.Count == 0)
                {
                    MessageBox.Show("No vehicles found", "Search Result", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching vehicles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        partial void OnSelectedVehicleChanged(Vehicle value)
        {
            if (value != null)
            {
                LoadVehicleHistory(value.VehicleID);
            }
        }

        private async void LoadVehicleHistory(int vehicleId)
        {
            try
            {
                var history = await _inspectionRepository.GetVehicleHistory(vehicleId);
                InspectionRecords = new ObservableCollection<InspectionRecord>(history);

                if (InspectionRecords.Count == 0)
                {
                    MessageBox.Show("No inspection history found", "History", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading history: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task LoadViolations()
        {
            try
            {
                // Tải danh sách vi phạm (xe hết hạn đăng kiểm hoặc không đạt)
                var violations = await _inspectionRepository.GetVehicleHistory(0); // 0 để lấy tất cả
                InspectionRecords = new ObservableCollection<InspectionRecord>(violations);

                if (InspectionRecords.Count == 0)
                {
                    MessageBox.Show("No violations found", "Violations", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading violations: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}