using FMS.ServiceLayer.DTO.TripDto;

namespace FMS.ServiceLayer.Interface
{
    public interface ITripService
    {
        Task<List<TripListDto>> GetTripsAsync();
        Task<TripStatsDto> GetTripStatsAsync();
        Task<OrderListDto> GetOrdersByIdAsync(int tripId);
        Task<List<BookedTripListDto>> GetBookedTripListAsync();
        Task<BookedTripStatsDto> GetBookedTripStatsAsync();

    }
}
