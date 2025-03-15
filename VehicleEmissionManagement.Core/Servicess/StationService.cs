using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Debug.WriteLine($"StationService.GetAppointmentsAsync called with stationId: {stationId}, date: {date}, status: {status}");
            try
            {
                var result = await _stationRepository.GetAppointmentsByStationIdAsync(stationId, date, status);
                Debug.WriteLine($"StationService.GetAppointmentsAsync returned {result.Count} appointments");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in StationService.GetAppointmentsAsync: {ex.Message}");
                throw; // Đảm bảo lỗi được truyền lên để xử lý ở ViewModel
            }
        }

        public async Task<bool> ConfirmAppointmentAsync(int appointmentId, int stationId)
        {
            Debug.WriteLine($"StationService.ConfirmAppointmentAsync called with appointmentId: {appointmentId}, stationId: {stationId}");
            try
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
                        Debug.WriteLine($"Notification created for appointment confirmation, UserId: {appointment.Vehicle.OwnerID}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in StationService.ConfirmAppointmentAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> RejectAppointmentAsync(int appointmentId, string reason)
        {
            Debug.WriteLine($"StationService.RejectAppointmentAsync called with appointmentId: {appointmentId}, reason: {reason}");
            try
            {
                var result = await _stationRepository.RejectAppointmentAsync(appointmentId, reason);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in StationService.RejectAppointmentAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetStatisticsAsync(int stationId, DateTime startDate, DateTime endDate)
        {
            Debug.WriteLine($"StationService.GetStatisticsAsync called with stationId: {stationId}, startDate: {startDate}, endDate: {endDate}");
            try
            {
                var result = await _stationRepository.GetInspectionStatisticsAsync(stationId, startDate, endDate);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in StationService.GetStatisticsAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<InspectionRecord>> GetInspectionHistoryAsync(int stationId, DateTime startDate, DateTime endDate)
        {
            Debug.WriteLine($"StationService.GetInspectionHistoryAsync called with stationId: {stationId}, startDate: {startDate}, endDate: {endDate}");
            try
            {
                var result = await _stationRepository.GetStationInspectionHistoryAsync(stationId, startDate, endDate);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in StationService.GetInspectionHistoryAsync: {ex.Message}");
                throw;
            }
        }
    }
}