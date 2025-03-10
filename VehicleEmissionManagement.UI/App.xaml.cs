using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Services;
using VehicleEmissionManagement.Data.Contextt;
using VehicleEmissionManagement.Data.Repositoriess;
using VehicleEmissionManagement.UI.Viewss;
using VehicleEmissionManagement.UI.ViewModelss;
using VehicleEmissionManagement.Core.Servicess;
using Microsoft.EntityFrameworkCore;

namespace VehicleEmissionManagement.UI
{
    public partial class App : Application
    {
        public ServiceProvider _serviceProvider;
        public static ServiceProvider Services => ((App)Current)._serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
            var loginView = new LoginView
            {
                DataContext = _serviceProvider.GetRequiredService<LoginViewModel>()
            };
            loginView.Show();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Database - sử dụng Transient để tránh xung đột
            services.AddDbContext<ApplicationDbContext>(options => { }, ServiceLifetime.Transient);

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IInspectionRepository, InspectionRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IOwnerService, OwnerService>();
            services.AddScoped<AppointmentService>();  // Thêm dịch vụ này

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<StationViewModel>();

            // Views
            services.AddTransient<PoliceDashboard>();
            services.AddTransient<StationDashboard>();
        }
    }
}