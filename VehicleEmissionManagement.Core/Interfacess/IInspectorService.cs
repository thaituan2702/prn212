using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Interfacess
{
    public interface IInspectorService
    {
        Task<List<InspectionRecord>> GetPendingInspectionsAsync(int inspectorId);
        Task<List<InspectionRecord>> GetCompletedInspectionsAsync(int inspectorId, DateTime? startDate = null, DateTime? endDate = null);
        Task<bool> SubmitInspectionResultAsync(InspectionRecord inspectionRecord);
        Task<List<InspectionRecord>> SearchInspectionsAsync(int inspectorId, string searchTerm);
        Task<Vehicle> GetVehicleDetailsAsync(int vehicleId);
        Task<List<Appointment>> GetTodayAppointmentsAsync(int stationId);
    }
}