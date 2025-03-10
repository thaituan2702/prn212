using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Data.Contextt;

namespace VehicleEmissionManagement.Data.Repositoriess
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Appointment>> GetAppointmentsByStationIdAsync(int stationId, DateTime? date = null, string status = null)
        {
            var query = _context.Appointments
                .Include(a => a.Vehicle)
                    .ThenInclude(v => v.Owner)
                .Where(a => a.StationID == stationId);

            if (date.HasValue)
            {
                query = query.Where(a => a.AppointmentDate.Date == date.Value.Date);
            }

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(a => a.Status == status);
            }

            return await query.OrderByDescending(a => a.AppointmentDate).ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByOwnerIdAsync(int ownerId)
        {
            return await _context.Appointments
                .Include(a => a.Vehicle)
                .Include(a => a.Station)
                .Where(a => a.Vehicle.OwnerID == ownerId)
                .ToListAsync();
        }

        public async Task<List<InspectionStation>> GetActiveStationsAsync()
        {
            return await _context.InspectionStations
                .Where(s => s.Status == "Active")
                .ToListAsync();
        }

        public async Task<bool> CreateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment != null)
            {
                appointment.Status = "Cancelled";
                appointment.UpdatedAt = DateTime.Now;
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }
    }
}