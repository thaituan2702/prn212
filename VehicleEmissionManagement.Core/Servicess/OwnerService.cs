using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Services
{
    public class OwnerService : IOwnerService
    {
        private readonly IUserRepository _userRepository;

        public OwnerService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> GetProfileAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<bool> UpdateProfileAsync(User user)
        {
            return await _userRepository.UpdateAsync(user);
        }
    }
}