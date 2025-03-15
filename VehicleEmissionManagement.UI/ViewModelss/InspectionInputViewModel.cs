using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Core.Servicess;

namespace VehicleEmissionManagement.UI.ViewModelss
{
    public partial class InspectionInputViewModel : ObservableObject
    {
        private readonly IInspectorService _inspectorService;
        private readonly Appointment _appointment;

        [ObservableProperty]
        private Vehicle _vehicle;

        [ObservableProperty]
        private decimal _co2Value;

        [ObservableProperty]
        private decimal _hcValue;

        [ObservableProperty]
        private decimal _noxValue;

        [ObservableProperty]
        private string _comments;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _result;

        // Thông số tiêu chuẩn khí thải
        public decimal CO2Standard { get; } = 2.50m;
        public decimal HCStandard { get; } = 0.10m;
        public decimal NOxStandard { get; } = 0.09m;

        public InspectionInputViewModel(IInspectorService inspectorService, Appointment appointment)
        {
            _inspectorService = inspectorService;
            _appointment = appointment;
            Vehicle = appointment.Vehicle;
            Result = "Pass"; // Mặc định là đạt

            // Load dữ liệu chi tiết xe
            LoadVehicleDetailsCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private async Task LoadVehicleDetails()
        {
            try
            {
                IsLoading = true;
                Debug.WriteLine($"Loading vehicle details for VehicleID: {_appointment.VehicleID}");

                var vehicleDetails = await _inspectorService.GetVehicleDetailsAsync(_appointment.VehicleID);
                if (vehicleDetails != null)
                {
                    Vehicle = vehicleDetails;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading vehicle details: {ex.Message}");
                MessageBox.Show($"Error loading vehicle details: {ex.Message}",
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
        private void CalculateResult()
        {
            // Đánh giá kết quả dựa trên các thông số khí thải
            if (Co2Value > CO2Standard || HcValue > HCStandard || NoxValue > NOxStandard)
            {
                Result = "Fail";
            }
            else
            {
                Result = "Pass";
            }
        }

        [RelayCommand]
        private async Task SaveInspection()
        {
            try
            {
                if (!ValidateInput())
                {
                    return;
                }

                IsLoading = true;
                Debug.WriteLine("Saving inspection result...");

                // Tạo bản ghi kiểm định mới
                var inspectionRecord = new InspectionRecord
                {
                    VehicleID = Vehicle.VehicleID,
                    StationID = _appointment.StationID,
                    InspectorID = AuthService.CurrentUser.UserID,
                    InspectionDate = DateTime.Now,
                    Result = Result,
                    CO2Emission = Co2Value,
                    HCEmission = HcValue,
                    NOxEmission = NoxValue,
                    Comments = Comments,
                    ExpiryDate = DateTime.Now.AddMonths(6),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var result = await _inspectorService.SubmitInspectionResultAsync(inspectionRecord);
                if (result)
                {
                    MessageBox.Show("Inspection result saved successfully!",
                                  "Success",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                    return;
                }
                else
                {
                    MessageBox.Show("Could not save inspection result. Please try again.",
                                  "Error",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving inspection result: {ex.Message}");
                MessageBox.Show($"Error saving inspection result: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool ValidateInput()
        {
            if (Co2Value <= 0)
            {
                MessageBox.Show("Please enter a valid CO2 emission value.",
                              "Validation Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return false;
            }

            if (HcValue <= 0)
            {
                MessageBox.Show("Please enter a valid HC emission value.",
                              "Validation Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return false;
            }

            if (NoxValue <= 0)
            {
                MessageBox.Show("Please enter a valid NOx emission value.",
                              "Validation Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}