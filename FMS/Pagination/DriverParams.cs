namespace FMS.Pagination
{
    public class DriverParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "DriverStatus";
        public bool IsDescending { get; set; } = true;

        // Các tiêu chí lọc
        public string? DriverStatus { get; set; }
       
    }
}
