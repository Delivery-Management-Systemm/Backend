using FMS.ServiceLayer.DTO.EmergencyReportDto;
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
        [HttpPost]
        public async Task<IActionResult> CreateEmergencyReport([FromBody] CreateEmergencyReportDto dto)
        {
            var createdReport = await _emergencyReportService.CreateEmergencyReportAsync(dto);
            return CreatedAtAction(nameof(GetAllEmergencyReports), new { id = createdReport.Id }, createdReport);
        }
        [HttpPut]
        [Route("respond")]
        public async Task<IActionResult> RespondEmergencyReport([FromBody] RespondEmergencyReportDto dto)
        {
            var respondedReport = await _emergencyReportService.RespondEmergencyReportAsync(dto);
            return Ok(respondedReport);
        }
        [HttpPut]
        [Route("resolve")]
        public async Task<IActionResult> ResolveEmergencyReport([FromBody] ResolveEmergencyReportDto dto)
        {
            var resolvedReport = await _emergencyReportService.ResolveEmergencyReportAsync(dto);
            return Ok(resolvedReport);
        }
        [HttpGet]
        [Route("stats")]
        public async Task<IActionResult> GetEmergencyReportStats()
        {
            var stats = await _emergencyReportService.GetEmergencyReportStatsAsync();
            return Ok(stats);
        }
    }
}
