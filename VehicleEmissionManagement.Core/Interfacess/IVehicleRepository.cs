using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Interfacess
{
    public interface IVehicleRepository
    {
        Task<List<Vehicle>> GetVehiclesByOwnerIdAsync(int ownerId);
        Task<Vehicle> GetVehicleByIdAsync(int id);
        Task<bool> CreateVehicleAsync(Vehicle vehicle);
        Task<bool> UpdateVehicleAsync(Vehicle vehicle);
        Task<IEnumerable<Vehicle>> SearchByPlateNumber(string plateNumber);
        Task<Vehicle> GetByPlateNumber(string plateNumber);
        Task<bool> DeleteVehicleAsync(int id);
    }
}