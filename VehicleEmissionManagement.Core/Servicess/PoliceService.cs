using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Interfaces;
using VehicleEmissionManagement.Core.Interfacess;

using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Services
{
    public class PoliceService : IPoliceService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IInspectionRepository _inspectionRepository;

        public PoliceService(
            IVehicleRepository vehicleRepository,
            IInspectionRepository inspectionRepository)
        {
            _vehicleRepository = vehicleRepository;
            _inspectionRepository = inspectionRepository;
        }

        public async Task<IEnumerable<Vehicle>> SearchVehicleByPlateNumber(string plateNumber)
        {
            return await _vehicleRepository.SearchByPlateNumber(plateNumber);
        }

        public async Task<Vehicle> GetVehicleDetail(int vehicleId)
        {
            return await _vehicleRepository.GetByPlateNumber(vehicleId.ToString());
        }

        public async Task<IEnumerable<InspectionRecord>> GetVehicleInspectionHistory(int vehicleId)
        {
            return await _inspectionRepository.GetVehicleHistory(vehicleId);
        }

        public async Task<bool> CheckVehicleViolation(string plateNumber)
        {
            var vehicle = await _vehicleRepository.GetByPlateNumber(plateNumber);
            if (vehicle == null) return false;

            var latestInspection = await _inspectionRepository.GetLatestInspection(vehicle.VehicleID);
            // Kiểm tra hết hạn bằng cách so sánh ExpiryDate với DateTime.Now
            return latestInspection == null || latestInspection.ExpiryDate < DateTime.Now;
        }
    }
    }
