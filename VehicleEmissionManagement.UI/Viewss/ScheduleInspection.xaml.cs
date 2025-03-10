using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Core.Servicess;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class ScheduleInspection : Window
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public ScheduleInspection()
        {
            InitializeComponent();
            _vehicleRepository = ((App)Application.Current)._serviceProvider.GetService<IVehicleRepository>();
            _appointmentRepository = ((App)Application.Current)._serviceProvider.GetService<IAppointmentRepository>();
            LoadInitialData();
        }

        private async void LoadInitialData()
        {
            try
            {
                // Load vehicles
                var vehicles = await _vehicleRepository.GetVehiclesByOwnerIdAsync(AuthService.CurrentUser.UserID);
                VehicleComboBox.ItemsSource = vehicles;

                // Load stations
                var stations = await _appointmentRepository.GetActiveStationsAsync();
                StationComboBox.ItemsSource = stations;

                // Set date restrictions
                DatePicker.DisplayDateStart = DateTime.Today.AddDays(1);
                DatePicker.DisplayDateEnd = DateTime.Today.AddMonths(1);
                DatePicker.SelectedDate = DateTime.Today.AddDays(1);

                // Load appointments
                await LoadAppointments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private async Task LoadAppointments()
        {
            try
            {
                var appointments = await _appointmentRepository.GetAppointmentsByOwnerIdAsync(AuthService.CurrentUser.UserID);
                AppointmentsGrid.ItemsSource = appointments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading appointments: {ex.Message}",
                             "Error",
                             MessageBoxButton.OK,
                             MessageBoxImage.Error);
            }
        }

        private async void ScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                var selectedVehicle = (Vehicle)VehicleComboBox.SelectedItem;
                var selectedStation = (InspectionStation)StationComboBox.SelectedItem;
                var selectedDate = DatePicker.SelectedDate.Value;
                var selectedTime = ((ComboBoxItem)TimeComboBox.SelectedItem).Content.ToString();

                var appointmentDateTime = DateTime.Parse($"{selectedDate.ToString("yyyy-MM-dd")} {selectedTime}");

                var appointment = new Appointment
                {
                    VehicleID = selectedVehicle.VehicleID,
                    StationID = selectedStation.StationID,
                    AppointmentDate = appointmentDateTime,
                    Status = "Pending",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                var result = await _appointmentRepository.CreateAppointmentAsync(appointment);
                if (result)
                {
                    MessageBox.Show("Appointment scheduled successfully!",
                                  "Success",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                    await LoadAppointments();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error scheduling appointment: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (VehicleComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a vehicle");
                return false;
            }

            if (StationComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select an inspection station");
                return false;
            }

            if (!DatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select a date");
                return false;
            }

            if (TimeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a time");
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            VehicleComboBox.SelectedItem = null;
            StationComboBox.SelectedItem = null;
            DatePicker.SelectedDate = DateTime.Today.AddDays(1);
            TimeComboBox.SelectedItem = null;
        }

        private async void CancelAppointment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var appointment = (Appointment)((FrameworkElement)sender).DataContext;

                var result = MessageBox.Show(
                    "Are you sure you want to cancel this appointment?",
                    "Confirm Cancel",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _appointmentRepository.CancelAppointmentAsync(appointment.AppointmentID);
                    await LoadAppointments();
                    MessageBox.Show("Appointment cancelled successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cancelling appointment: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}