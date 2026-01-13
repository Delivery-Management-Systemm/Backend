using FMS.Pagination;
using FMS.ServiceLayer.DTO.VehicleDto;
using FMS.ServiceLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    public class VehicleController: ControllerBase
    {
        private readonly IVehicleService _vehicleService;
        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }


        [HttpGet]
        public async Task<IActionResult> GetVehicles([FromQuery] VehicleParams @params)
        {
            try
            {
                var vehicles = await _vehicleService.GetVehiclesAsync(@params);
                return Ok(vehicles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpGet("{vehicleId}")]
        public async Task<IActionResult> GetVehicleDetails(int vehicleId)
        {
            try
            {
                var vehicleDetails = await _vehicleService.GetVehicleDetailsAsync(vehicleId);
                if (vehicleDetails == null)
                {
                    return NotFound(new { message = "Vehicle not found" });
                }
                return Ok(vehicleDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody] VehicleCreateDto dto)
        {
            try
            {
                var createdVehicle = await _vehicleService.CreateVehicleAsync(dto);
                return CreatedAtAction(nameof(GetVehicleDetails), new { vehicleId = createdVehicle.VehicleID }, createdVehicle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
