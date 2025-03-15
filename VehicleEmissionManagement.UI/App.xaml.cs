using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Services;
using VehicleEmissionManagement.Data.Contextt;
using VehicleEmissionManagement.Data.Repositoriess;
using VehicleEmissionManagement.UI.Viewss;
using VehicleEmissionManagement.UI.ViewModelss;
using VehicleEmissionManagement.Core.Servicess;
using Microsoft.Extensions.Configuration;
using System;

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
            // Đọc connection string từ appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            // Database - Đăng ký DbContext sử dụng factory để tạo instance mới mỗi khi cần
            services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionStringDB") + ";MultipleActiveResultSets=true"));

            // Đăng ký DbContext thông thường với lifetime là transient
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnectionStringDB") + ";MultipleActiveResultSets=true"),
                ServiceLifetime.Transient);

            // Repositories
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IVehicleRepository, VehicleRepository>();
            services.AddTransient<IAppointmentRepository, AppointmentRepository>();
            services.AddTransient<IInspectionRepository, InspectionRepository>();
            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddTransient<IStationRepository, StationRepository>();

            // Services
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IOwnerService, OwnerService>();
            services.AddTransient<IStationService, StationService>();

            // ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<PoliceDashboard>();
            services.AddTransient<StationViewModel>();
            services.AddTransient<StationReportsViewModel>();
        }
    }
}