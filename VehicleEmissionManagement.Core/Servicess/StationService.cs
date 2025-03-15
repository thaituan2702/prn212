using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Servicess
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
            Debug.WriteLine($"StationService.GetAppointmentsAsync gọi với stationId: {stationId}, date: {date}, status: {status}");
            try
            {
                // Kiểm tra xem có dữ liệu nào cho StationID này không (không lọc)
                var allAppointments = await _stationRepository.GetAllAppointmentsAsync();
                var appointmentsForStation = allAppointments.FindAll(a => a.StationID == stationId);
                Debug.WriteLine($"Tổng số lịch hẹn trong DB: {allAppointments.Count}");
                Debug.WriteLine($"Số lịch hẹn cho StationID {stationId}: {appointmentsForStation.Count}");

                // Nếu không có StationID trong DB, thông báo cho người dùng
                if (appointmentsForStation.Count == 0)
                {
                    Debug.WriteLine($"CẢNH BÁO: Không có lịch hẹn nào cho StationID={stationId} trong cơ sở dữ liệu!");
                }

                // Truy vấn bình thường
                var result = await _stationRepository.GetAppointmentsByStationIdAsync(stationId, date, status);
                Debug.WriteLine($"StationService.GetAppointmentsAsync trả về {result.Count} lịch hẹn");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong StationService.GetAppointmentsAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Đảm bảo lỗi được truyền lên để xử lý ở ViewModel
            }
        }

        public async Task<bool> ConfirmAppointmentAsync(int appointmentId, int stationId)
        {
            Debug.WriteLine($"StationService.ConfirmAppointmentAsync gọi với appointmentId: {appointmentId}, stationId: {stationId}");
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
                            Message = $"Lịch đăng kiểm của bạn vào ngày {appointment.AppointmentDate:dd/MM/yyyy HH:mm} đã được xác nhận.",
                            Type = "Info",
                            SentDate = DateTime.Now,
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        };

                        await _notificationRepository.CreateNotificationAsync(notification);
                        Debug.WriteLine($"Đã tạo thông báo cho xác nhận lịch hẹn, UserId: {appointment.Vehicle.OwnerID}");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong StationService.ConfirmAppointmentAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<bool> RejectAppointmentAsync(int appointmentId, string reason)
        {
            Debug.WriteLine($"StationService.RejectAppointmentAsync gọi với appointmentId: {appointmentId}, reason: {reason}");
            try
            {
                var result = await _stationRepository.RejectAppointmentAsync(appointmentId, reason);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong StationService.RejectAppointmentAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetStatisticsAsync(int stationId, DateTime startDate, DateTime endDate)
        {
            Debug.WriteLine($"StationService.GetStatisticsAsync gọi với stationId: {stationId}, startDate: {startDate}, endDate: {endDate}");
            try
            {
                var result = await _stationRepository.GetInspectionStatisticsAsync(stationId, startDate, endDate);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong StationService.GetStatisticsAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<List<InspectionRecord>> GetInspectionHistoryAsync(int stationId, DateTime startDate, DateTime endDate)
        {
            Debug.WriteLine($"StationService.GetInspectionHistoryAsync gọi với stationId: {stationId}, startDate: {startDate}, endDate: {endDate}");
            try
            {
                var result = await _stationRepository.GetStationInspectionHistoryAsync(stationId, startDate, endDate);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong StationService.GetInspectionHistoryAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}