using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleEmissionManagement.Core.Modelss
{
    public class Appointment
    {
        public int AppointmentID { get; set; }
        public int VehicleID { get; set; }
        public int StationID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } // Pending, Confirmed, Completed, Cancelled
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Vehicle Vehicle { get; set; }
        public InspectionStation Station { get; set; }
    }
}
