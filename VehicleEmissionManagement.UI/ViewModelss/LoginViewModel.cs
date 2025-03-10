using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.UI.Viewss;

namespace VehicleEmissionManagement.UI.ViewModelss
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly IOwnerService _ownerService;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IInspectionRepository _inspectionRepository;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        public LoginViewModel(
            IAuthService authService,
            IOwnerService ownerService,
            IVehicleRepository vehicleRepository,
            IInspectionRepository inspectionRepository)
        {
            _authService = authService;
            _ownerService = ownerService;
            _vehicleRepository = vehicleRepository;
            _inspectionRepository = inspectionRepository;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
                {
                    MessageBox.Show("Vui lòng nhập email và mật khẩu");
                    return;
                }

                Debug.WriteLine($"Attempting login with email: {Email}");
                var user = await _authService.LoginAsync(Email, Password);
                Debug.WriteLine($"Login result: {user != null}");

                if (user != null)
                {
                    Window dashboardWindow = user.Role switch
                    {
                        "Owner" => new OwnerDashboard(_ownerService, _authService),
                        "Station" => new StationDashboard(),
                        "Inspector" => new InspectorDashboard(),
                        "Police" => new PoliceDashboard(_vehicleRepository, _inspectionRepository),
                        "Admin" => new AdminDashboard(),
                        _ => throw new ArgumentException("Role không hợp lệ")
                    };

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is LoginView)
                        {
                            window.Close();
                        }
                    }

                    dashboardWindow.Show();
                }
                else
                {
                    MessageBox.Show("Email hoặc mật khẩu không đúng!");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Login error: {ex}");
                MessageBox.Show($"Lỗi đăng nhập: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Register()
        {
            var registerWindow = new RegisterView();
            registerWindow.Show();
            Application.Current.Windows.OfType<LoginView>().FirstOrDefault()?.Close();
        }
    }
}