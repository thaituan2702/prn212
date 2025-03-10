using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace VehicleEmissionManagement.Core.Modelss
{
    public class Notification
    {
        public int NotificationID { get; set; }
        public int UserID { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }  // Reminder, Alert, Info
        public DateTime SentDate { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public User User { get; set; }
    }
}
