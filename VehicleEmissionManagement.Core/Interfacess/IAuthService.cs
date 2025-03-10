using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Interfacess
{
    public interface IAuthService
    {
        Task<User> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(User user);
        void Logout();
    }
}
