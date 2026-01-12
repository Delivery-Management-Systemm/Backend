namespace FMS.ServiceLayer.DTO.EmergencyReportDto
{
    public class EmergencyReportStatsDto
    {
        public int newReports { get; set; }
        public int processing { get; set; }
        public int resolved { get; set; }
        public int critical { get; set; }
    }
}
