using FMS.Pagination;
using FMS.ServiceLayer.DTO.MaintenanceDto;
using FMS.ServiceLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceController: ControllerBase
    {
        private readonly IMaintenanceService _maintenanceService;
        public MaintenanceController(IMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }
        [HttpGet("services")]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _maintenanceService.GetAllServiceAsync();
            return Ok(services);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetMaintenanceStats()
        {
            var stats = await _maintenanceService.GetMaintenanceStatsAsync();
            return Ok(stats);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllInvoices([FromQuery] MaintenanceParams @params)
        {
            var invoices = await _maintenanceService.GetAllInvoiceAsync(@params);
            return Ok(invoices);
        }
        [HttpPost]
        public async Task<IActionResult> CreateMaintenance([FromBody] CreateMaintenanceDto dto)
        {
            var maintenanceId = await _maintenanceService.CreateMaintenanceAsync(dto);
            return CreatedAtAction(nameof(GetAllInvoices), new { id = maintenanceId }, new { Id = maintenanceId });
        }
    }
}
