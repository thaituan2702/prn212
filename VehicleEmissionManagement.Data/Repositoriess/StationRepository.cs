using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Data.Contextt;

namespace VehicleEmissionManagement.Data.Repositoriess
{
    public class StationRepository : IStationRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public StationRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Appointment>> GetAppointmentsByStationIdAsync(int stationId, DateTime? date = null, string status = null)
        {
            Debug.WriteLine($"StationRepository.GetAppointmentsByStationIdAsync gọi với stationId: {stationId}, date: {date}, status: {status}");
            try
            {
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    // In thông tin kết nối DB để kiểm tra
                    Debug.WriteLine($"Đang sử dụng connection string: {context.Database.GetConnectionString()}");

                    // Log để kiểm tra ngày tháng
                    if (date.HasValue)
                    {
                        Debug.WriteLine($"Tìm kiếm lịch hẹn vào ngày {date.Value:yyyy-MM-dd}, thành phần thời gian: {date.Value:HH:mm:ss.fff}");
                    }

                    // Truy vấn cơ bản - KHÔNG ÁP DỤNG TRACKING để tránh lỗi
                    var query = context.Appointments
                        .AsNoTracking()
                        .Include(a => a.Vehicle)
                            .ThenInclude(v => v.Owner)
                        .Include(a => a.Station)
                        .Where(a => a.StationID == stationId);

                    // Kiểm tra và in ra tổng số lịch hẹn theo StationID (không lọc)
                    var allAppointmentsForStation = await query.ToListAsync();
                    Debug.WriteLine($"Tổng số lịch hẹn cho StationID {stationId} (không lọc): {allAppointmentsForStation.Count}");

                    // In thông tin chi tiết về từng lịch hẹn
                    foreach (var app in allAppointmentsForStation)
                    {
                        Debug.WriteLine($"ID: {app.AppointmentID}, Ngày: {app.AppointmentDate:yyyy-MM-dd HH:mm:ss}, Trạng thái: {app.Status}");
                    }

                    // Xử lý ngày đặc biệt
                    if (date.HasValue)
                    {
                        // Chuyển sang query giữa đầu ngày và cuối ngày để truy vấn chính xác hơn
                        var startOfDay = date.Value.Date;
                        var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

                        Debug.WriteLine($"Khoảng ngày: {startOfDay:yyyy-MM-dd HH:mm:ss.fff} đến {endOfDay:yyyy-MM-dd HH:mm:ss.fff}");

                        // Thực hiện truy vấn trực tiếp để kiểm tra
                        var sqlDate = date.Value.ToString("yyyy-MM-dd");
                        var testSql = $"SELECT * FROM Appointments WHERE StationID = {stationId} AND CONVERT(date, AppointmentDate) = '{sqlDate}'";
                        Debug.WriteLine($"Test SQL query: {testSql}");

                        // Áp dụng điều kiện vào query
                        query = query.Where(a => a.AppointmentDate.Date == startOfDay.Date);

                        // Kiểm tra SQL và số lượng kết quả sau khi lọc theo ngày
                        var dateFilteredCount = await query.CountAsync();
                        Debug.WriteLine($"Số lịch hẹn sau khi lọc theo ngày: {dateFilteredCount}");
                    }

                    // Xử lý status
                    if (!string.IsNullOrEmpty(status) && status != "All")
                    {
                        query = query.Where(a => a.Status == status);

                        // Kiểm tra SQL và số lượng kết quả sau khi lọc theo status
                        var statusFilteredCount = await query.CountAsync();
                        Debug.WriteLine($"Số lịch hẹn sau khi lọc theo status: {statusFilteredCount}");
                    }

                    // Thực hiện truy vấn
                    var appointments = await query.OrderByDescending(a => a.AppointmentDate).ToListAsync();

                    // Log kết quả để debug
                    Debug.WriteLine($"Tìm thấy {appointments.Count} lịch hẹn");
                    foreach (var app in appointments)
                    {
                        Debug.WriteLine($"Lịch hẹn ID: {app.AppointmentID}, Ngày: {app.AppointmentDate:dd/MM/yyyy HH:mm}, Trạng thái: {app.Status}, Xe: {app.Vehicle?.PlateNumber ?? "N/A"}");
                    }

                    return appointments;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong StationRepository.GetAppointmentsByStationIdAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw new Exception($"Lỗi khi lấy danh sách lịch hẹn: {ex.Message}", ex);
            }
        }

        // Phương thức mới để lấy tất cả lịch hẹn không lọc theo StationID
        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            try
            {
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    var allAppointments = await context.Appointments
                        .AsNoTracking()
                        .Include(a => a.Vehicle)
                            .ThenInclude(v => v.Owner)
                        .Include(a => a.Station)
                        .ToListAsync();

                    Debug.WriteLine($"Tổng số lịch hẹn trong cơ sở dữ liệu: {allAppointments.Count}");
                    return allAppointments;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi khi lấy tất cả lịch hẹn: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ConfirmAppointmentAsync(int appointmentId)
        {
            try
            {
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    var appointment = await context.Appointments.FindAsync(appointmentId);
                    if (appointment != null)
                    {
                        appointment.Status = "Confirmed";
                        appointment.UpdatedAt = DateTime.Now;
                        var result = await context.SaveChangesAsync();
                        return result > 0;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xác nhận lịch hẹn: {ex.Message}", ex);
            }
        }

        public async Task<bool> RejectAppointmentAsync(int appointmentId, string reason)
        {
            try
            {
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    var appointment = await context.Appointments.FindAsync(appointmentId);
                    if (appointment != null)
                    {
                        appointment.Status = "Cancelled";
                        appointment.UpdatedAt = DateTime.Now;

                        // Tìm OwnerID
                        int ownerId = context.Vehicles
                            .Where(v => v.VehicleID == appointment.VehicleID)
                            .Select(v => v.OwnerID)
                            .FirstOrDefault();

                        // Tạo thông báo cho chủ xe
                        var notification = new Notification
                        {
                            UserID = ownerId,
                            Title = "Lịch đăng kiểm bị từ chối",
                            Message = $"Lịch đăng kiểm của bạn vào ngày {appointment.AppointmentDate.ToString("dd/MM/yyyy HH:mm")} đã bị từ chối. Lý do: {reason}",
                            Type = "Alert",
                            SentDate = DateTime.Now,
                            IsRead = false,
                            CreatedAt = DateTime.Now
                        };

                        context.Notifications.Add(notification);
                        var result = await context.SaveChangesAsync();
                        return result > 0;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi từ chối lịch hẹn: {ex.Message}", ex);
            }
        }

        public async Task<List<InspectionRecord>> GetStationInspectionHistoryAsync(int stationId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    return await context.InspectionRecords
                        .AsNoTracking()
                        .Include(i => i.Vehicle)
                            .ThenInclude(v => v.Owner)
                        .Include(i => i.Inspector)
                        .Where(i => i.StationID == stationId
                                   && i.InspectionDate >= startDate
                                   && i.InspectionDate <= endDate)
                        .OrderByDescending(i => i.InspectionDate)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy lịch sử kiểm định: {ex.Message}", ex);
            }
        }

        public async Task<Dictionary<string, int>> GetInspectionStatisticsAsync(int stationId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    var inspections = await context.InspectionRecords
                        .AsNoTracking()
                        .Where(i => i.StationID == stationId
                                   && i.InspectionDate >= startDate
                                   && i.InspectionDate <= endDate)
                        .ToListAsync();

                    return new Dictionary<string, int>
                    {
                        { "TotalInspections", inspections.Count },
                        { "PassCount", inspections.Count(i => i.Result == "Pass") },
                        { "FailCount", inspections.Count(i => i.Result == "Fail") }
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê kiểm định: {ex.Message}", ex);
            }
        }

        public async Task<InspectionStation> GetStationByIdAsync(int stationId)
        {
            try
            {
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    return await context.InspectionStations
                        .AsNoTracking()
                        .FirstOrDefaultAsync(s => s.StationID == stationId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin trạm: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateStationInfoAsync(InspectionStation station)
        {
            try
            {
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    var existingStation = await context.InspectionStations.FindAsync(station.StationID);
                    if (existingStation != null)
                    {
                        context.Entry(existingStation).CurrentValues.SetValues(station);
                        existingStation.UpdatedAt = DateTime.Now;

                        var result = await context.SaveChangesAsync();
                        return result > 0;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật thông tin trạm: {ex.Message}", ex);
            }
        }
    }
}