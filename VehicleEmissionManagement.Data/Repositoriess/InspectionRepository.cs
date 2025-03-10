using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Data.Contextt;

namespace VehicleEmissionManagement.Data.Repositoriess
{
    public class InspectionRepository : IInspectionRepository
    {
        private readonly ApplicationDbContext _context;

        public InspectionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<InspectionRecord>> GetInspectionHistoryByVehicleIdAsync(int vehicleId)
        {
            try
            {
                var query = await _context.InspectionRecords
                    .AsNoTracking()  // Thêm dòng này để tránh tracking changes
                    .Include(i => i.Station)
                    .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Owner)
                    .Where(i => i.VehicleID == vehicleId)
                    .OrderByDescending(i => i.InspectionDate)
                    .ToListAsync();

                return query;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting inspection history: {ex.Message}");
            }
        }

        public async Task<IEnumerable<InspectionRecord>> GetVehicleHistory(int vehicleId)
        {
            try
            {
                var result = await GetInspectionHistoryByVehicleIdAsync(vehicleId);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting vehicle history: {ex.Message}");
            }
        }

        public async Task<InspectionRecord> GetLatestInspection(int vehicleId)
        {
            return await _context.InspectionRecords
                .Include(i => i.Station)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Owner)
                .Where(i => i.VehicleID == vehicleId)
                .OrderByDescending(i => i.InspectionDate)
                .FirstOrDefaultAsync();
        }

        public async Task<InspectionRecord> GetInspectionByIdAsync(int recordId)
        {
            return await _context.InspectionRecords
                .Include(i => i.Station)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Owner)
                .FirstOrDefaultAsync(i => i.RecordID == recordId);
        }

        public async Task<bool> CreateInspectionAsync(InspectionRecord record)
        {
            try
            {
                _context.InspectionRecords.Add(record);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateInspectionAsync(InspectionRecord record)
        {
            try
            {
                _context.InspectionRecords.Update(record);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<InspectionRecord>> GetLatestInspectionsAsync(int vehicleId, int count = 1)
        {
            return await _context.InspectionRecords
                .Include(i => i.Station)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Owner)
                .Where(i => i.VehicleID == vehicleId)
                .OrderByDescending(i => i.InspectionDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> HasValidInspectionAsync(int vehicleId)
        {
            var latestInspection = await GetLatestInspection(vehicleId);
            if (latestInspection == null) return false;

            return latestInspection.ExpiryDate > DateTime.Now;
        }

        public async Task<List<InspectionRecord>> GetInspectionsByStationIdAsync(int stationId)
        {
            return await _context.InspectionRecords
                .Include(i => i.Station)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Owner)
                .Where(i => i.StationID == stationId)
                .OrderByDescending(i => i.InspectionDate)
                .ToListAsync();
        }

        public async Task<List<InspectionRecord>> GetInspectionsByDateRangeAsync(int stationId, DateTime startDate, DateTime endDate)
        {
            return await _context.InspectionRecords
                .Include(i => i.Station)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Owner)
                .Where(i => i.StationID == stationId
                           && i.InspectionDate >= startDate
                           && i.InspectionDate <= endDate)
                .OrderByDescending(i => i.InspectionDate)
                .ToListAsync();
        }

        public async Task<List<InspectionRecord>> GetPendingInspectionsAsync(int stationId)
        {
            var today = DateTime.Today;
            return await _context.InspectionRecords
                .Include(i => i.Station)
                .Include(i => i.Vehicle)
                    .ThenInclude(v => v.Owner)
                .Where(i => i.StationID == stationId
                           && i.InspectionDate.Date == today)
                .OrderBy(i => i.InspectionDate)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetInspectionStatisticsAsync(int stationId, DateTime startDate, DateTime endDate)
        {
            var inspections = await _context.InspectionRecords
                .Where(i => i.StationID == stationId
                           && i.InspectionDate >= startDate
                           && i.InspectionDate <= endDate)
                .ToListAsync();

            return new Dictionary<string, int>
            {
                { "Total", inspections.Count },
                { "Pass", inspections.Count(i => i.Result == "Pass") },
                { "Fail", inspections.Count(i => i.Result == "Fail") }
            };
        }
    }
}