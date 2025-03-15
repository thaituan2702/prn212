using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Interfacess
{
    public interface IStationService
    {
        Task<List<Appointment>> GetAppointmentsAsync(int stationId, DateTime? date = null, string status = null);
        Task<bool> ConfirmAppointmentAsync(int appointmentId, int stationId);
        Task<bool> RejectAppointmentAsync(int appointmentId, string reason);
        Task<Dictionary<string, int>> GetStatisticsAsync(int stationId, DateTime startDate, DateTime endDate);
        Task<List<InspectionRecord>> GetInspectionHistoryAsync(int stationId, DateTime startDate, DateTime endDate);
    }
}