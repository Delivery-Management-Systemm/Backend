using FMS.Pagination;
using FMS.ServiceLayer.DTO.EmergencyReportDto;
using FMS.ServiceLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[EnableRateLimiting("fixed")]
    public class EmergencyReportController: ControllerBase
    {
        private readonly IEmergencyReportService _emergencyReportService;
        public EmergencyReportController(IEmergencyReportService emergencyReportService)
        {
            _emergencyReportService = emergencyReportService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllEmergencyReports([FromQuery] EmergencyReportParams @params)
        {
            var reports = await _emergencyReportService.GetAllAsync(@params);
            return Ok(reports);
        }
        [HttpPost]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> CreateEmergencyReport([FromBody] CreateEmergencyReportDto dto)
        {
            var createdReport = await _emergencyReportService.CreateEmergencyReportAsync(dto);
            return CreatedAtAction(nameof(GetAllEmergencyReports), new { id = createdReport.Id }, createdReport);
        }
        [HttpPut]
        [Route("respond")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> RespondEmergencyReport([FromBody] RespondEmergencyReportDto dto)
        {
            var respondedReport = await _emergencyReportService.RespondEmergencyReportAsync(dto);
            return Ok(respondedReport);
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
