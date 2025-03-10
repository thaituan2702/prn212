using VehicleEmissionManagement.Core.Modelss;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Interfaces
{
    public interface IPoliceService
    {
        Task<IEnumerable<Vehicle>> SearchVehicleByPlateNumber(string plateNumber);
        Task<Vehicle> GetVehicleDetail(int vehicleId);
        Task<IEnumerable<InspectionRecord>> GetVehicleInspectionHistory(int vehicleId);
        Task<bool> CheckVehicleViolation(string plateNumber);
    }
}