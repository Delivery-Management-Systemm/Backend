namespace FMS.Pagination
{
    public class TripParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "StartTime";
        public bool IsDescending { get; set; } = true;

        // Các tiêu chí lọc
        public string? TripStatus { get; set; }
    }
}
