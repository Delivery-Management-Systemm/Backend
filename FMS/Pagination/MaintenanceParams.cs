namespace FMS.Pagination
{
    public class MaintenanceParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "MaintenanceType";
        public bool IsDescending { get; set; } = true;
    }
}
