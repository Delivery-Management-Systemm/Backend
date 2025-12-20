using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FMS.Models
{
    public class Trip
    {
        [Key]
        public int TripID { get; set; }

        // ====== KHÓA NGOẠI ======

        [ForeignKey(nameof(Vehicle))]
        public int VehicleID { get; set; }
        public Vehicle Vehicle { get; set; }

        [ForeignKey(nameof(Driver))]
        public int DriverID { get; set; }
        public Driver Driver { get; set; }

        // ====== THÔNG TIN CHUYẾN ======

        // ====== THỜI GIAN ======
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }   // null nếu chưa xong

        // ====== LỘ TRÌNH ======
        [Required, StringLength(255)]
        public string StartLocation { get; set; }

        [Required, StringLength(255)]
        public string EndLocation { get; set; }

        // ====== THỐNG KÊ ======
        public int? TotalDistanceKm { get; set; }
        public double? TotalFuelConsumed { get; set; }

        [Required, StringLength(20)]
        public string TripStatus { get; set; }
        // planned | in-progress | completed | canceled

        public ICollection<FuelRecord>? FuelRecords { get; set; }
        public ICollection<ExtraExpense>? ExtraExpenses { get; set; }
        public ICollection<TripLog>? TripLogs { get; set; }
    }
}
