using FMS.Pagination;
using FMS.ServiceLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    public class TripController: ControllerBase
    {
        private readonly ITripService _tripService;
        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTripsAsync([FromQuery] TripParams @params)
        {
            var trips = await _tripService.GetTripsAsync(@params);
            return Ok(trips);
        }
        [HttpGet("stats")]
        public async Task<IActionResult> GetTripStatsAsync()
        {
            // Placeholder for future implementation
            var stats = await _tripService.GetTripStatsAsync();
            return Ok(stats);
        }
        [HttpGet("/{tripId}/orders")]
        public async Task<IActionResult> GetOrdersAsync(int tripId)
        {
            var order = await _tripService.GetOrdersByIdAsync(tripId);
            return Ok(order);
        }
        [HttpGet("booked")]
        public async Task<IActionResult> GetBookedTripsAsync([FromQuery] BookedTripParams @params)
        {
            var bookedTrips = await _tripService.GetBookedTripListAsync(@params);
            return Ok(bookedTrips);
        }
        [HttpGet("booked/stats")]
        public async Task<IActionResult> GetBookedTripStatsAsync()
        {
            var bookedTripStats = await _tripService.GetBookedTripStatsAsync();
            return Ok(bookedTripStats);
        }
    }
}
