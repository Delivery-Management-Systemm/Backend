using FMS.ServiceLayer.DTO.DriverDto;
using FMS.ServiceLayer.Implementation;
using FMS.ServiceLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    public class DriverController: ControllerBase
    {
        private readonly IDriverService _driverService;
        public DriverController(IDriverService driverService)
        {
            _driverService = driverService;
        }


        [HttpGet]
        public async Task<IActionResult> GetDrivers()
        {
            try
            {
                var drivers = await _driverService.GetDriversAsync();
                return Ok(drivers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("{driverId}")]
        public async Task<IActionResult> GetDriverDetails(int driverId)
        {
            try
            {
                var driverDetails = await _driverService.GetDriverDetailsAsync(driverId);
                return Ok(driverDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("{driverId}/history")]
        public async Task<IActionResult> GetDriverHistory(int driverId)
        {
            try
            {
                var history = await _driverService.GetDriverHistoryAsync(driverId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateDriver([FromBody] CreateDriverDto dto)
        {
            var result = await _driverService.CreateDriverAsync(dto);
            return Ok(result);
        }
    }
}
