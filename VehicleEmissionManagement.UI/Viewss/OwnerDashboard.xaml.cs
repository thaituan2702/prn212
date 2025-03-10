using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Core.Servicess;
using VehicleEmissionManagement.Data.Repositoriess;
using VehicleEmissionManagement.UI.ViewModelss;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class OwnerDashboard : Window
    {
        private readonly IOwnerService _ownerService;
        private readonly IAuthService _authService;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IAppointmentRepository _appointmentRepository; // Thêm ở phần khai báo
        private readonly IInspectionRepository _inspectionRepository;

        public OwnerDashboard(IOwnerService ownerService, IAuthService authService)
        {
            InitializeComponent();
            _ownerService = ownerService;
            _authService = authService;
            // Lấy IVehicleRepository từ DI
            _vehicleRepository = ((App)Application.Current)._serviceProvider.GetService<IVehicleRepository>();
            _appointmentRepository = ((App)Application.Current)._serviceProvider.GetService<IAppointmentRepository>(); // Thêm dòng này
            _inspectionRepository = ((App)Application.Current)._serviceProvider.GetService<IInspectionRepository>();

        }

        private void ProfileManagement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var profileWindow = new ProfileManagement(_ownerService);
                profileWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening profile management: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var authService = ((App)Application.Current)._serviceProvider.GetService<IAuthService>();
            authService.Logout();
            var loginWindow = new LoginView
            {
                DataContext = ((App)Application.Current)._serviceProvider.GetRequiredService<LoginViewModel>()
            };
            loginWindow.Show();
            Close();
        }

        private async void MyVehicles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Hide welcome panel and show vehicles panel
                WelcomePanel.Visibility = Visibility.Collapsed;
                VehiclesPanel.Visibility = Visibility.Visible;

                // Load vehicles data
                var vehicles = await _vehicleRepository.GetVehiclesByOwnerIdAsync(AuthService.CurrentUser.UserID);
                VehiclesGrid.ItemsSource = vehicles;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vehicles: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private async void AddVehicle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new VehicleDialog(_vehicleRepository);
                if (dialog.ShowDialog() == true)
                {
                    await RefreshVehiclesList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding vehicle: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private async void EditVehicle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var vehicle = (Vehicle)((FrameworkElement)sender).DataContext;
                var dialog = new VehicleDialog(_vehicleRepository, vehicle);
                if (dialog.ShowDialog() == true)
                {
                    await RefreshVehiclesList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing vehicle: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private async void DeleteVehicle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var vehicle = (Vehicle)((FrameworkElement)sender).DataContext;
                var result = MessageBox.Show(
                    $"Are you sure you want to delete vehicle {vehicle.PlateNumber}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await _vehicleRepository.DeleteVehicleAsync(vehicle.VehicleID);
                    await RefreshVehiclesList();
                    MessageBox.Show("Vehicle deleted successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting vehicle: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private async Task RefreshVehiclesList()
        {
            var vehicles = await _vehicleRepository.GetVehiclesByOwnerIdAsync(AuthService.CurrentUser.UserID);
            VehiclesGrid.ItemsSource = vehicles;
        }

        private void Schedule_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var scheduleWindow = new ScheduleInspection();
                scheduleWindow.Owner = this;
                scheduleWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening scheduling window: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private async void ScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateScheduleInput())
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
                    MessageBox.Show("Appointment scheduled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearScheduleForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error scheduling appointment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            ClearScheduleForm();
            SchedulePanel.Visibility = Visibility.Collapsed;
            WelcomePanel.Visibility = Visibility.Visible;
        }

        private bool ValidateScheduleInput()
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

        private void ClearScheduleForm()
        {
            VehicleComboBox.SelectedItem = null;
            StationComboBox.SelectedItem = null;
            DatePicker.SelectedDate = DateTime.Today.AddDays(1);
            TimeComboBox.SelectedItem = null;
        }
        private async void InspectionHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kiểm tra xem có vehicle được chọn không
                if (VehiclesGrid.SelectedItem is Vehicle selectedVehicle)
                {
                    var historyWindow = new InspectionHistory(selectedVehicle);
                    historyWindow.Owner = this;
                    historyWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Please select a vehicle first",
                                 "Information",
                                 MessageBoxButton.OK,
                                 MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening inspection history: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void Notifications_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var notificationWindow = new NotificationWindow();
                notificationWindow.Owner = this;
                notificationWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening notifications: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
    }
}