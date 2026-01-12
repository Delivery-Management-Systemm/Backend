using FMS.ServiceLayer.DTO.EmergencyReportDto;

namespace FMS.ServiceLayer.Interface
{
    public interface IEmergencyReportService
    {
        Task<List<EmergencyReportListDto>> GetAllAsync();
        Task<EmergencyReportListDto> CreateEmergencyReportAsync(CreateEmergencyReportDto dto);
        Task<EmergencyReportListDto> RespondEmergencyReportAsync(RespondEmergencyReportDto dto);
        Task<EmergencyReportListDto> ResolveEmergencyReportAsync(ResolveEmergencyReportDto dto);
        Task<EmergencyReportStatsDto> GetEmergencyReportStatsAsync();
    }
}
