namespace FMS.ServiceLayer.DTO.TripDto
{
    public class OrderListDto
    {
        public int Id { get; set; }                 // TripID
        public string? Customer { get; set; }
        public string? Contact { get; set; }

        public string Pickup { get; set; }
        public string Dropoff { get; set; }

        public string Vehicle { get; set; }         // LicensePlate
        public string Driver { get; set; }          // Driver name

        public string Status { get; set; }          // waiting | in_transit | delivered

        public List<TripStepDto> Steps { get; set; }

        public string Cost { get; set; }             // "800,000đ"
    }
}
