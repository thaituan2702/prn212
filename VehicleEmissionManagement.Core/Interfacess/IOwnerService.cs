using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Interfacess
{
    public interface IOwnerService
    {
        Task<User> GetProfileAsync(int userId);
        Task<bool> UpdateProfileAsync(User user);
    }
}