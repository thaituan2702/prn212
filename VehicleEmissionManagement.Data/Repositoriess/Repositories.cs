using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Interfacess;
using VehicleEmissionManagement.Core.Modelss;
using VehicleEmissionManagement.Data.Contextt;

namespace VehicleEmissionManagement.Data.Repositoriess
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ApplicationDbContext _context;

        public VehicleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Vehicle>> GetVehiclesByOwnerIdAsync(int ownerId)
        {
            return await _context.Vehicles
                .Where(v => v.OwnerID == ownerId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Vehicle>> SearchByPlateNumber(string plateNumber)
        {
            return await _context.Vehicles
                .Where(v => v.PlateNumber.Contains(plateNumber))
                .ToListAsync();
        }

        public async Task<Vehicle> GetByPlateNumber(string plateNumber)
        {
            return await _context.Vehicles
                .FirstOrDefaultAsync(v => v.PlateNumber == plateNumber);
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            return await _context.Vehicles
                .FirstOrDefaultAsync(v => v.VehicleID == id);
        }

        public async Task<bool> CreateVehicleAsync(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateVehicleAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteVehicleAsync(int id)
        {
            var vehicle = await GetVehicleByIdAsync(id);
            if (vehicle == null) return false;

            _context.Vehicles.Remove(vehicle);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}