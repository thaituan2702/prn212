using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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

        [ObservableProperty]
        private bool isEmailError;

        [ObservableProperty]
        private string emailErrorMessage;

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

            // Khởi tạo giá trị mặc định
            IsEmailError = false;
            EmailErrorMessage = "Email phải có định dạng @gmail.com";
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            try
            {
                // Reset trạng thái lỗi
                IsEmailError = false;

                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
                {
                    MessageBox.Show("Vui lòng nhập email và mật khẩu");
                    return;
                }

                // Kiểm tra định dạng email phải là gmail.com
                if (!IsValidGmailAddress(Email))
                {
                    // Đặt flag hiển thị lỗi thay vì hiện MessageBox
                    IsEmailError = true;
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

        // Hàm kiểm tra email có phải định dạng @gmail.com không
        private bool IsValidGmailAddress(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Kiểm tra email có định dạng hợp lệ và kết thúc bằng @gmail.com
            string pattern = @"^[a-zA-Z0-9._%+-]+@gmail\.com$";
            return Regex.IsMatch(email, pattern);
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