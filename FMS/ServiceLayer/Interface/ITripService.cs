using FMS.Pagination;
using FMS.ServiceLayer.DTO.TripDto;

namespace FMS.ServiceLayer.Interface
{
    public interface ITripService
    {
        Task<PaginatedResult<TripListDto>> GetTripsAsync(TripParams @params);
        Task<TripStatsDto> GetTripStatsAsync();
        Task<OrderListDto> GetOrdersByIdAsync(int tripId);
        Task<PaginatedResult<BookedTripListDto>> GetBookedTripListAsync(BookedTripParams @params);
        Task<BookedTripStatsDto> GetBookedTripStatsAsync();

    }
}
