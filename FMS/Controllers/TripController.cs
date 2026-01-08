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
        public async Task<IActionResult> GetTripsAsync()
        {
            var trips = await _tripService.GetTripsAsync();
            return Ok(trips);
        }
        [HttpGet("/stats")]
        public async Task<IActionResult> GetTripStatsAsync()
        {
            // Placeholder for future implementation
            var stats = await _tripService.GetTripStatsAsync();
            return Ok(stats);
        }
        [HttpGet("/orders")]
        public async Task<IActionResult> GetOrdersAsync()
        {
            var orders = await _tripService.GetOrdersAsync();
            return Ok(orders);
        }
    }
}
