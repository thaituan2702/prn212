using System.Diagnostics;
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
            Debug.WriteLine("AppointmentManagementView initialized");
        }

        private async void AppointmentManagementView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Debug.WriteLine("AppointmentManagementView Loaded event fired");
            if (DataContext is StationViewModel viewModel)
            {
                // Đặt ngày chính xác để tìm kiếm
                // Nếu muốn tìm ngày 15/3/2025 thì set như sau:
                viewModel.SelectedDate = new DateTime(2025, 3, 15);
                viewModel.SelectedStatus = "All"; // Không lọc theo status

                Debug.WriteLine($"Loading data with date: {viewModel.SelectedDate:dd/MM/yyyy}, status: {viewModel.SelectedStatus}");

                // Load dữ liệu khi view đã được tải hoàn toàn
                await viewModel.LoadAppointmentsCommand.ExecuteAsync(null);
            }
            else
            {
                Debug.WriteLine($"DataContext is not StationViewModel, it is: {DataContext?.GetType().Name ?? "null"}");
            }
        }
    }
}