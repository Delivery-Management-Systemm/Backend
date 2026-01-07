using FMS.ServiceLayer.DTO.EmergencyReportDto;

namespace FMS.ServiceLayer.Interface
{
    public interface IEmergencyReportService
    {
        Task<List<EmergencyReportListDto>> GetAllAsync();
    }
}
