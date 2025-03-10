using System;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Core.Servicess;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class VehicleDialog : Window
    {
        private readonly IVehicleRepository _vehicleRepository;
        private Vehicle _vehicle;
        private bool _isEditMode;

        public Vehicle Vehicle => _vehicle;

        public VehicleDialog(IVehicleRepository vehicleRepository, Vehicle vehicle = null)
        {
            InitializeComponent();
            _vehicleRepository = vehicleRepository;
            _isEditMode = vehicle != null;
            _vehicle = vehicle ?? new Vehicle
            {
                OwnerID = AuthService.CurrentUser.UserID,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            DataContext = _vehicle;
            this.Title = _isEditMode ? "Edit Vehicle" : "Add New Vehicle";
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    if (_isEditMode)
                    {
                        _vehicle.UpdatedAt = DateTime.Now;
                        var result = await _vehicleRepository.UpdateVehicleAsync(_vehicle);
                        if (result)
                        {
                            MessageBox.Show("Vehicle updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            DialogResult = true;
                        }
                    }
                    else
                    {
                        var result = await _vehicleRepository.CreateVehicleAsync(_vehicle);
                        if (result)
                        {
                            MessageBox.Show("Vehicle added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            DialogResult = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving vehicle: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(_vehicle.PlateNumber))
            {
                MessageBox.Show("Please enter plate number", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(_vehicle.Brand))
            {
                MessageBox.Show("Please enter brand", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(_vehicle.Model))
            {
                MessageBox.Show("Please enter model", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (_vehicle.ManufactureYear <= 0)
            {
                MessageBox.Show("Please enter a valid manufacture year", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(_vehicle.EngineNumber))
            {
                MessageBox.Show("Please enter engine number", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}