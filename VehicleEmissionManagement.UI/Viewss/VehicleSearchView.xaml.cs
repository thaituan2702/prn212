using System;
using System.Windows;
using System.Windows.Controls;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class VehicleSearchView : UserControl
    {
        public VehicleSearchView()
        {
            InitializeComponent();
        }

        private void ViewHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var vehicle = (Vehicle)((FrameworkElement)sender).DataContext;
                var historyWindow = new InspectionHistory(vehicle);
                if (Window.GetWindow(this) is Window parentWindow)
                {
                    historyWindow.Owner = parentWindow;
                }
                historyWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error viewing history: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
    }
}