using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.UI.ViewModelss;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class ProfileManagement : Window
    {
        public ProfileManagement(IOwnerService ownerService)
        {
            InitializeComponent();
            DataContext = new OwnerProfileViewModel(ownerService);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}