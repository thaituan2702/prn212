using System.Diagnostics;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
namespace VehicleEmissionManagement.Core.Servicess
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        public static User CurrentUser { get; set; }
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            // Thêm log để debug
            Debug.WriteLine($"Login attempt - Email: {email}, User found: {user != null}");
            if (user == null) return null;
            if (user.Password != password) return null;
            CurrentUser = user;
            return user;
        }

        public async Task<bool> RegisterAsync(User user)
        {
            return await _userRepository.CreateAsync(user);
        }

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}