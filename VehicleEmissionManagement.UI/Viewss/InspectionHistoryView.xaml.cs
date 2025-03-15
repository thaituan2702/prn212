using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Servicess;
using VehicleEmissionManagement.UI.ViewModelss;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class InspectionHistoryView : Window
    {
        private readonly InspectionHistoryViewModel _viewModel;

        public InspectionHistoryView()
        {
            InitializeComponent();

            // Lấy service từ DI container
            var inspectorService = ((App)Application.Current)._serviceProvider.GetService<IInspectorService>();

            // Nếu service chưa được đăng ký, tạo mới
            if (inspectorService == null)
            {
                var inspectionRepository = ((App)Application.Current)._serviceProvider.GetService<IInspectionRepository>();
                var vehicleRepository = ((App)Application.Current)._serviceProvider.GetService<IVehicleRepository>();
                var appointmentRepository = ((App)Application.Current)._serviceProvider.GetService<IAppointmentRepository>();
                inspectorService = new InspectorService(inspectionRepository, vehicleRepository, appointmentRepository);
            }

            // Tạo và gán ViewModel
            _viewModel = new InspectionHistoryViewModel(inspectorService);
            DataContext = _viewModel;
        }
    }
}