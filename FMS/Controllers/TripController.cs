using FMS.Pagination;
using FMS.ServiceLayer.DTO.TripDto;
using FMS.ServiceLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
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
        [HttpGet("{tripId}/orders")]
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
        [HttpPost("booked")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> CreateBookingTripAsync([FromBody] CreateBookingTripDto dto)
        {
            var createdTrip = await _tripService.CreateBookingTripAsync(dto);
            return Ok(createdTrip);
        }

        // GET: api/trip/options/statuses
        [HttpGet("options/statuses")]
        public IActionResult GetTripStatuses()
        {
            var statuses = new[]
            {
                new { value = "waiting", label = "Chờ xử lý" },
                new { value = "confirmed", label = "Đã xác nhận" },
                new { value = "in_transit", label = "Đang thực hiện" },
                new { value = "completed", label = "Hoàn thành" }
            };
            return Ok(statuses);
        }

        // PUT: api/trip/booked/{tripId}/cancel
        [HttpPut("booked/{tripId}/cancel")]
        public async Task<IActionResult> CancelBookedTrip(int tripId)
        {
            var result = await _tripService.CancelBookedTripAsync(tripId);
            return result ? Ok(new { message = "Đã hủy lịch đặt trước" }) : NotFound(new { message = "Không tìm thấy lịch đặt trước" });
        }

        // DELETE: api/trip/booked/{tripId}
        [HttpDelete("booked/{tripId}")]
        public async Task<IActionResult> DeleteBookedTrip(int tripId)
        {
            var result = await _tripService.DeleteBookedTripAsync(tripId);
            return result ? Ok(new { message = "Đã xóa lịch đặt trước" }) : NotFound(new { message = "Không tìm thấy lịch đặt trước" });
        }
    }
}
