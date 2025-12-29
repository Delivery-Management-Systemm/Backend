using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMS.Models
{
    public class Vehicle
    {
        [Key] public int VehicleID { get; set; }
        [Required, StringLength(100)] public string LicensePlate { get; set; }
        [StringLength(100)] public string? VehicleModel { get; set; }

        [StringLength(100)]public string? VehicleType { get; set; }
        public int? ManufacturedYear { get; set; }
        public int? CurrentKm { get; set; }
        [StringLength(20)]  public string? VehicleStatus { get; set; }
        [ForeignKey("Driver")] public int? DriverID { get; set; } //driver hien tai
        public Driver? Driver { get; set; }

        public ICollection<Maintenance>? Maintenances { get; set; }
        
        
        // ====== LICENSE REQUIREMENT ======
        [ForeignKey(nameof(RequiredLicenseClass))]
        public int RequiredLicenseClassID { get; set; }
        public LicenseClass RequiredLicenseClass { get; set; }

        public ICollection<VehicleDriverAssignment>? VehicleAssignments { get; set; }
        public ICollection<Trip>? Trips { get; set; }

        public ICollection<FuelRecord>? FuelRecords { get; set; }
        public ICollection<EmergencyReport> EmergencyReports { get; set; }


    }
}
