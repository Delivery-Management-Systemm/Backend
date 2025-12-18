using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FMS.Models
{
    public class VehicleDriverAssignment
    {
        [Key]
        public int AssignmentID { get; set; }

        [ForeignKey("Driver")] public int DriverID { get; set; }
        [ForeignKey("Vehicle")] public int VehicleID { get; set; }

        public DateTime AssignedFrom { get; set; }
        public DateTime? AssignedTo { get; set; }

 
        public Vehicle Vehicle { get; set; }

        public Driver Driver { get; set; }
    }
}
