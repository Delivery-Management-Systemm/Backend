namespace FMS.Pagination
{
    public class BookedTripParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "StartTime";
        public bool IsDescending { get; set; } = true;
    }
}
