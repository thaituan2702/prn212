using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Services;
using VehicleEmissionManagement.Data.Contextt;
using VehicleEmissionManagement.Data.Repositoriess;
using VehicleEmissionManagement.UI.Viewss;
using VehicleEmissionManagement.UI.ViewModelss;
using VehicleEmissionManagement.Core.Servicess;

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
            // Database
            services.AddDbContext<ApplicationDbContext>();

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IOwnerService, OwnerService>();  // Thêm dòng này

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IInspectionRepository, InspectionRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddTransient<PoliceDashboard>();
        }
    }
}