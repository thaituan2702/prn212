using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            Debug.WriteLine($"StationRepository.GetAppointmentsByStationIdAsync called with stationId: {stationId}, date: {date}, status: {status}");
            try
            {
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    // Log để kiểm tra ngày tháng
                    if (date.HasValue)
                    {
                        Debug.WriteLine($"Looking for appointments on {date.Value:yyyy-MM-dd}, time component: {date.Value:HH:mm:ss.fff}");
                    }

                    // Truy vấn cơ bản
                    var query = context.Appointments
                        .AsNoTracking()
                        .Include(a => a.Vehicle)
                            .ThenInclude(v => v.Owner)
                        .Include(a => a.Station)
                        .Where(a => a.StationID == stationId);

                    // Xử lý ngày đặc biệt
                    if (date.HasValue)
                    {
                        // Chuyển sang query giữa đầu ngày và cuối ngày để truy vấn chính xác hơn
                        var startOfDay = date.Value.Date;
                        var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

                        Debug.WriteLine($"Date range: {startOfDay:yyyy-MM-dd HH:mm:ss.fff} to {endOfDay:yyyy-MM-dd HH:mm:ss.fff}");

                        query = query.Where(a => a.AppointmentDate >= startOfDay && a.AppointmentDate <= endOfDay);
                    }

                    // Xử lý status
                    if (!string.IsNullOrEmpty(status) && status != "All")
                    {
                        query = query.Where(a => a.Status == status);
                    }

                    // Thực hiện truy vấn
                    var appointments = await query.OrderByDescending(a => a.AppointmentDate).ToListAsync();

                    // Log kết quả để debug
                    Debug.WriteLine($"Found {appointments.Count} appointments");
                    foreach (var app in appointments)
                    {
                        Debug.WriteLine($"Appointment ID: {app.AppointmentID}, Date: {app.AppointmentDate:dd/MM/yyyy HH:mm}, Status: {app.Status}, Vehicle: {app.Vehicle?.PlateNumber ?? "N/A"}");
                    }

                    return appointments;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in StationRepository.GetAppointmentsByStationIdAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw new Exception($"Lỗi khi lấy danh sách lịch hẹn: {ex.Message}", ex);
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
        public async Task<string> TestDirectQuery(int stationId, DateTime date)
        {
            try
            {
                using (var context = await _contextFactory.CreateDbContextAsync())
                {
                    var startOfDay = date.Date;
                    var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

                    // Tạo câu SQL để kiểm tra
                    var sql = $@"
                        SELECT COUNT(*) 
                        FROM Appointments 
                        WHERE StationID = {stationId} 
                        AND AppointmentDate >= '{startOfDay:yyyy-MM-dd HH:mm:ss.fff}' 
                        AND AppointmentDate <= '{endOfDay:yyyy-MM-dd HH:mm:ss.fff}'";

                    // Thực hiện raw SQL query
                    var count = await context.Database.ExecuteSqlRawAsync(sql);

                    return $"Query executed. Count: {count}";
                }
            }
            catch (Exception ex)
            {
                return $"Error executing direct query: {ex.Message}";
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