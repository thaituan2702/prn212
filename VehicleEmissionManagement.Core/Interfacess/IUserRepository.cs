using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Interfacess
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<bool> CreateAsync(User user);
        Task<List<User>> GetAllAsync();
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<User> GetByIdAsync(int id);
    }
}
