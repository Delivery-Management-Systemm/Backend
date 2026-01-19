using FMS.ServiceLayer.DTO.DashboardDto;

namespace FMS.ServiceLayer.Interface
{
    public interface IStatService
    {
        Task<DashboardStatDto> GetDashboardStatsAsync();
    }
}
