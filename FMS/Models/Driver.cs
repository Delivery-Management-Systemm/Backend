using System.ComponentModel.DataAnnotations;

namespace FMS.Models
{
    public class Driver
    {
        [Key] public int DriverID { get; set; }

        [Required, StringLength(200)] public string FullName { get; set; }
        [StringLength(20)] public string Phone { get; set; }
        [Required,StringLength(50)] public string LicenseNumber { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public int TotalTrips { get; set; }
        public double Rating { get; set; }
        [StringLength(20)] public string? DriverStatus { get; set; }

        // Lịch sử gán xe
        public ICollection<VehicleDriverAssignment>? VehicleAssignments { get; set; }
        public ICollection<Trip>? Trips { get; set; }
        public ICollection<FuelRecord>? FuelRecords { get; set; }


    }
}
