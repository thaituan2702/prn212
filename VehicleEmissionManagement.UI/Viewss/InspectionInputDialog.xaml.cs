using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Core.Servicess;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class InspectionInputDialog : Window
    {
        private readonly IInspectionRepository _inspectionRepository;
        private readonly Appointment _appointment;

        public InspectionInputDialog(Appointment appointment)
        {
            InitializeComponent();
            _appointment = appointment;
            _inspectionRepository = ((App)Application.Current)._serviceProvider.GetService<IInspectionRepository>();
            DataContext = _appointment;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput())
                {
                    MessageBox.Show("Please fill in all required fields with valid values.");
                    return;
                }

                var inspection = new InspectionRecord
                {
                    VehicleID = _appointment.VehicleID,
                    StationID = _appointment.StationID,
                    InspectorID = AuthService.CurrentUser.UserID,
                    InspectionDate = DateTime.Now,
                    CO2Emission = decimal.Parse(CO2TextBox.Text),
                    HCEmission = decimal.Parse(HCTextBox.Text),
                    NOxEmission = decimal.Parse(NOxTextBox.Text),
                    Comments = CommentsTextBox.Text,
                    Result = DetermineResult(),
                    ExpiryDate = DateTime.Now.AddMonths(6),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var result = await _inspectionRepository.CreateInspectionAsync(inspection);
                if (result)
                {
                    _appointment.Status = "Completed";
                    await ((App)Application.Current)._serviceProvider.GetService<IAppointmentRepository>()
                        .UpdateAppointmentAsync(_appointment);

                    MessageBox.Show("Inspection results saved successfully!");
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving inspection results: {ex.Message}");
            }
        }

        private bool ValidateInput()
        {
            return !string.IsNullOrWhiteSpace(CO2TextBox.Text) &&
                   !string.IsNullOrWhiteSpace(HCTextBox.Text) &&
                   !string.IsNullOrWhiteSpace(NOxTextBox.Text) &&
                   decimal.TryParse(CO2TextBox.Text, out _) &&
                   decimal.TryParse(HCTextBox.Text, out _) &&
                   decimal.TryParse(NOxTextBox.Text, out _);
        }

        private string DetermineResult()
        {
            // Add your logic to determine Pass/Fail based on emission values
            var co2 = decimal.Parse(CO2TextBox.Text);
            var hc = decimal.Parse(HCTextBox.Text);
            var nox = decimal.Parse(NOxTextBox.Text);

            // Example thresholds (adjust according to your requirements)
            return co2 <= 5.0m && hc <= 300 && nox <= 50 ? "Pass" : "Fail";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}