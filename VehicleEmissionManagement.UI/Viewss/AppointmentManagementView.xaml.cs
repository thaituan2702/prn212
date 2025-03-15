using System.Windows.Controls;
using VehicleEmissionManagement.UI.ViewModelss;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class AppointmentManagementView : UserControl
    {
        public AppointmentManagementView()
        {
            InitializeComponent();

            // Kích hoạt load dữ liệu sau khi control đã được khởi tạo
            this.Loaded += AppointmentManagementView_Loaded;
        }

        private async void AppointmentManagementView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is StationViewModel viewModel)
            {
                // Load dữ liệu khi view đã được tải hoàn toàn
                await viewModel.LoadAppointmentsCommand.ExecuteAsync(null);
            }
        }
    }
}