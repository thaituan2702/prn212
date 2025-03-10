using System.Windows;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.UI.Viewss
{
    public partial class InspectionDetailsDialog : Window
    {
        public InspectionDetailsDialog(InspectionRecord record)
        {
            InitializeComponent();
            DataContext = record;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}