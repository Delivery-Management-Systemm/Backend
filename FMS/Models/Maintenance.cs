using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMS.Models
{
    public class Maintenance
    {
        [Key] public int MaintenanceID { get; set; }
        [ForeignKey("Vehicle")] public int VehicleID { get; set; }
        public Vehicle Vehicle { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? FinishedDate { get; set; }
        public string Description { get; set; }
        public double? Cost { get; set; }
        [StringLength(20)] public string? MaintenanceStatus { get; set; }

    }
}
