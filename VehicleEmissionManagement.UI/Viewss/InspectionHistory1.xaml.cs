// InspectionHistory.xaml.cs
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class InspectionHistory : Window
    {
        private readonly Vehicle _vehicle;
        private readonly IInspectionRepository _inspectionRepository;

        public InspectionHistory(Vehicle vehicle)
        {
            InitializeComponent();
            _vehicle = vehicle;
            _inspectionRepository = ((App)Application.Current)._serviceProvider.GetService<IInspectionRepository>();
            LoadHistoryData();
        }

        private async void LoadHistoryData()
        {
            var history = await _inspectionRepository.GetVehicleHistory(_vehicle.VehicleID);
            HistoryGrid.ItemsSource = history;
        }
    }
}