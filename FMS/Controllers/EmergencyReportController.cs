using FMS.ServiceLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    public class EmergencyReportController: ControllerBase
    {
        private readonly IEmergencyReportService _emergencyReportService;
        public EmergencyReportController(IEmergencyReportService emergencyReportService)
        {
            _emergencyReportService = emergencyReportService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllEmergencyReports()
        {
            var reports = await _emergencyReportService.GetAllAsync();
            return Ok(reports);
        }
    }
}
