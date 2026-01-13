namespace FMS.Pagination
{
    public class VehicleParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "LicensePlate";
        public bool IsDescending { get; set; } = true;

        // Các tiêu chí lọc
        public string? FuelType { get; set; }
        public string? VehicleBrand { get; set; }
        public string? VehicleStatus { get; set; }
        
    }
}
