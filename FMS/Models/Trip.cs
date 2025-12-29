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
        // planned | waiting | in_transit | delivered | canceled


        // ====== KHÁCH HÀNG ======
        [StringLength(200)]
        public string? CustomerName { get; set; }

        [StringLength(20)]
        public string? CustomerPhone { get; set; }

        public ICollection<FuelRecord>? FuelRecords { get; set; }
        public ICollection<ExtraExpense>? ExtraExpenses { get; set; }
        public ICollection<TripLog>? TripLogs { get; set; }
        public ICollection<TripDriver> TripDrivers { get; set; }
        public ICollection<TripStep>? TripSteps { get; set; }
        public ICollection<EmergencyReport> EmergencyReports { get; set; }

    }
}
