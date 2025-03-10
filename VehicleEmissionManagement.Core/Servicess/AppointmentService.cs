using System;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Servicess
{
    public class AppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task ConfirmAppointmentAsync(int appointmentId, int stationId)
        {
            var appointment = await _appointmentRepository.GetAppointmentsByStationIdAsync(stationId)
                .ContinueWith(t => t.Result.Find(a => a.AppointmentID == appointmentId));
            if (appointment != null && appointment.StationID == stationId && appointment.Status == "Pending")
            {
                appointment.Status = "Confirmed";
                appointment.UpdatedAt = DateTime.Now;
                await _appointmentRepository.UpdateAppointmentAsync(appointment);
            }
        }

        public async Task CancelAppointmentAsync(int appointmentId, int stationId)
        {
            var appointment = await _appointmentRepository.GetAppointmentsByStationIdAsync(stationId)
                .ContinueWith(t => t.Result.Find(a => a.AppointmentID == appointmentId));
            if (appointment != null && appointment.StationID == stationId && appointment.Status != "Cancelled")
            {
                appointment.Status = "Cancelled";
                appointment.UpdatedAt = DateTime.Now;
                await _appointmentRepository.UpdateAppointmentAsync(appointment);
            }
        }

        public async Task ScheduleInspectionAsync(int appointmentId, DateTime newDate, int stationId)
        {
            var appointment = await _appointmentRepository.GetAppointmentsByStationIdAsync(stationId)
                .ContinueWith(t => t.Result.Find(a => a.AppointmentID == appointmentId));
            if (appointment != null && appointment.StationID == stationId)
            {
                appointment.AppointmentDate = newDate;
                appointment.Status = "Pending";
                appointment.UpdatedAt = DateTime.Now;
                await _appointmentRepository.UpdateAppointmentAsync(appointment);
            }
        }
    }
}