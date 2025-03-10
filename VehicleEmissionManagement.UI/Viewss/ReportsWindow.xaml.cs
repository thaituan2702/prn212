// ReportsWindow.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class ReportsWindow : Window
    {
        private readonly IInspectionRepository _inspectionRepository;

        public ReportsWindow()
        {
            InitializeComponent();
            _inspectionRepository = ((App)Application.Current)._serviceProvider.GetService<IInspectionRepository>();

            // Set default dates
            StartDate.SelectedDate = DateTime.Now.AddMonths(-1);
            EndDate.SelectedDate = DateTime.Now;
        }

        private async void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!StartDate.SelectedDate.HasValue || !EndDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("Please select date range", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // TODO: Implement report generation logic
                // var reportData = await _inspectionRepository.GetReportData(StartDate.SelectedDate.Value, EndDate.SelectedDate.Value);
                // ReportGrid.ItemsSource = reportData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}