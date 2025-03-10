namespace VehicleEmissionManagement.Core.Modelss
{
    public class Vehicle
    {
        public int VehicleID { get; set; }
        public int OwnerID { get; set; }
        public string PlateNumber { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int ManufactureYear { get; set; }
        public string EngineNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public User Owner { get; set; }
    }
}