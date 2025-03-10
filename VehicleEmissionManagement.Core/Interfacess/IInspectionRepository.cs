using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Interfacess
{
    public interface IInspectionRepository
    {
        Task<List<InspectionRecord>> GetInspectionHistoryByVehicleIdAsync(int vehicleId);
        Task<InspectionRecord> GetInspectionByIdAsync(int recordId);
        Task<bool> CreateInspectionAsync(InspectionRecord record);
        Task<bool> UpdateInspectionAsync(InspectionRecord record);
        Task<List<InspectionRecord>> GetLatestInspectionsAsync(int vehicleId, int count = 1);
        Task<bool> HasValidInspectionAsync(int vehicleId);


        Task<List<InspectionRecord>> GetInspectionsByStationIdAsync(int stationId);
        Task<Dictionary<string, int>> GetInspectionStatisticsAsync(int stationId, DateTime startDate, DateTime endDate);
        Task<List<InspectionRecord>> GetInspectionsByDateRangeAsync(int stationId, DateTime startDate, DateTime endDate);
        Task<List<InspectionRecord>> GetPendingInspectionsAsync(int stationId);

        Task<IEnumerable<InspectionRecord>> GetVehicleHistory(int vehicleId);
        Task<InspectionRecord> GetLatestInspection(int vehicleId);
    }
}