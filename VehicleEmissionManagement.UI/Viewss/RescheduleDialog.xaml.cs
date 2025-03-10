using System;
using System.Windows;
using System.Windows.Controls;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class RescheduleDialog : Window
    {
        private readonly Appointment _appointment;

        public DateTime SelectedDateTime { get; private set; }

        public RescheduleDialog(Appointment appointment)
        {
            InitializeComponent();
            _appointment = appointment;
            DataContext = appointment;

            // Set default values
            NewDatePicker.DisplayDateStart = DateTime.Today.AddDays(1);
            NewDatePicker.DisplayDateEnd = DateTime.Today.AddMonths(1);
            NewDatePicker.SelectedDate = DateTime.Today.AddDays(1);

            // Select first time slot by default
            if (NewTimeComboBox.Items.Count > 0)
            {
                NewTimeComboBox.SelectedIndex = 0;
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (!NewDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewTimeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn giờ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedDate = NewDatePicker.SelectedDate.Value;
            var selectedTime = ((ComboBoxItem)NewTimeComboBox.SelectedItem).Content.ToString();

            // Combine date and time
            SelectedDateTime = DateTime.Parse($"{selectedDate.ToString("yyyy-MM-dd")} {selectedTime}");

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}