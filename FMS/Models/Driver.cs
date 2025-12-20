using System.ComponentModel.DataAnnotations;

namespace FMS.Models
{
    public class Driver
    {
        [Key] public int DriverID { get; set; }

        [Required, StringLength(200)] public string FullName { get; set; }
        [StringLength(20)] public string Phone { get; set; }
        
        public int TotalTrips { get; set; }
        public double Rating { get; set; }

        // Active | OnTrip | OnLeave | Suspended
        [StringLength(20)] public string? DriverStatus { get; set; }

        // Lịch sử gán xe
        public ICollection<VehicleDriverAssignment>? VehicleAssignments { get; set; }
        public ICollection<TripDriver>? TripDrivers { get; set; }

        public ICollection<FuelRecord>? FuelRecords { get; set; }
        public ICollection<DriverLicense>? DriverLicenses { get; set; }


    }
}
