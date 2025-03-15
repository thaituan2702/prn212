using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Interfacess
{
    public interface IStationRepository
    {
        Task<List<Appointment>> GetAllAppointmentsAsync();
        Task<List<Appointment>> GetAppointmentsByStationIdAsync(int stationId, DateTime? date = null, string status = null);
        Task<bool> ConfirmAppointmentAsync(int appointmentId);
        Task<bool> RejectAppointmentAsync(int appointmentId, string reason);
        Task<List<InspectionRecord>> GetStationInspectionHistoryAsync(int stationId, DateTime startDate, DateTime endDate);
        Task<Dictionary<string, int>> GetInspectionStatisticsAsync(int stationId, DateTime startDate, DateTime endDate);
        Task<InspectionStation> GetStationByIdAsync(int stationId);
        Task<bool> UpdateStationInfoAsync(InspectionStation station);
    }
}