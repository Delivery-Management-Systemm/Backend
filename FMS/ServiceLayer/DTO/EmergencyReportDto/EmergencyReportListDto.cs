namespace FMS.ServiceLayer.DTO.EmergencyReportDto
{
    public class EmergencyReportListDto
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Level { get; set; }    // high | critical
        public string Status { get; set; }   // new | processing | resolved

        public string Desc { get; set; }

        public string Location { get; set; }
        public string Contact { get; set; }

        public string Reporter { get; set; }
        public string Driver { get; set; }
        public string Vehicle { get; set; }

        public string ReportedAt { get; set; }
        public string? RespondedAt { get; set; }
        public string? ResolvedAt { get; set; }
    }
}
