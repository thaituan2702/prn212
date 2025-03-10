using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace VehicleEmissionManagement.Core.Modelss
{
    public class InspectionRecord
    {
        public int RecordID { get; set; }
        public int VehicleID { get; set; }
        public int StationID { get; set; }
        public int InspectorID { get; set; }
        public DateTime InspectionDate { get; set; }
        public string Result { get; set; }  // Pass, Fail
        public decimal CO2Emission { get; set; }
        public decimal HCEmission { get; set; }
        public decimal NOxEmission { get; set; }
        public string Comments { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Vehicle Vehicle { get; set; }
        public InspectionStation Station { get; set; }
        public User Inspector { get; set; }
    }
}