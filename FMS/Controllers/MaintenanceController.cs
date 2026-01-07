using FMS.ServiceLayer.Interface;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            var invoices = await _maintenanceService.GetAllInvoiceAsync();
            return Ok(invoices);
        }
    }
}
