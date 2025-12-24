using FMS.ServiceLayer.Implementation;
using FMS.ServiceLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
    }
}
