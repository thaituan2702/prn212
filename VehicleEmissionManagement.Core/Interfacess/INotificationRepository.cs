using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Core.Interfacess
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
        Task<bool> CreateNotificationAsync(Notification notification);
        Task<bool> UpdateNotificationAsync(Notification notification);
        Task<bool> DeleteNotificationAsync(int notificationId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteAllReadAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
    }
}