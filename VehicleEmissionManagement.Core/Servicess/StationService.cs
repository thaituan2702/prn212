using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Services
{
    public class StationService : IStationService
    {
        private readonly IStationRepository _stationRepository;
        private readonly INotificationRepository _notificationRepository;

        public StationService(IStationRepository stationRepository, INotificationRepository notificationRepository)
        {
            _stationRepository = stationRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<List<Appointment>> GetAppointmentsAsync(int stationId, DateTime? date = null, string status = null)
        {
            return await _stationRepository.GetAppointmentsByStationIdAsync(stationId, date, status);
        }

        public async Task<bool> ConfirmAppointmentAsync(int appointmentId, int stationId)
        {
            var result = await _stationRepository.ConfirmAppointmentAsync(appointmentId);

            if (result)
            {
                // Lấy thông tin cuộc hẹn
                var appointments = await _stationRepository.GetAppointmentsByStationIdAsync(stationId);
                var appointment = appointments.Find(a => a.AppointmentID == appointmentId);

                if (appointment != null)
                {
                    // Gửi thông báo cho chủ xe
                    var notification = new Notification
                    {
                        UserID = appointment.Vehicle.OwnerID,
                        Title = "Lịch đăng kiểm được xác nhận",
                        Message = $"Lịch đăng kiểm của bạn vào ngày {appointment.AppointmentDate.ToString("dd/MM/yyyy HH:mm")} đã được xác nhận.",
                        Type = "Info",
                        SentDate = DateTime.Now,
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    };

                    await _notificationRepository.CreateNotificationAsync(notification);
                }
            }

            return result;
        }

        public async Task<bool> RejectAppointmentAsync(int appointmentId, string reason)
        {
            return await _stationRepository.RejectAppointmentAsync(appointmentId, reason);
        }

        public async Task<Dictionary<string, int>> GetStatisticsAsync(int stationId, DateTime startDate, DateTime endDate)
        {
            return await _stationRepository.GetInspectionStatisticsAsync(stationId, startDate, endDate);
        }

        public async Task<List<InspectionRecord>> GetInspectionHistoryAsync(int stationId, DateTime startDate, DateTime endDate)
        {
            return await _stationRepository.GetStationInspectionHistoryAsync(stationId, startDate, endDate);
        }
    }
}