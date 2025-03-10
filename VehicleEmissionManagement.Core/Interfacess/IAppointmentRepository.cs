using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Interfacess
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAppointmentsByOwnerIdAsync(int ownerId);
        Task<List<InspectionStation>> GetActiveStationsAsync();
        Task<bool> CreateAppointmentAsync(Appointment appointment);
        Task<bool> UpdateAppointmentAsync(Appointment appointment);
        Task<bool> CancelAppointmentAsync(int appointmentId);
        Task<List<Appointment>> GetAppointmentsByStationIdAsync(int stationId, DateTime? date = null, string status = null);

    }
}