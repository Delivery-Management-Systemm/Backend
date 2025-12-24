namespace FMS.ServiceLayer.DTO.DriverDto
{
    public class DriverListDto
    {
        public int DriverID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public string LicenseNumber { get; set; } = "N/A";
        public DateTime? LicenseExpiry { get; set; }

        public string AssignedVehicle { get; set; } = "Chưa gán";

        public int TotalTrips { get; set; }
        public double Rating { get; set; }

        public string Status { get; set; } = "Active";
    }
}
