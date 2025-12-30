using FMS.ServiceLayer.DTO.DriverDto;

namespace FMS.ServiceLayer.Interface
{
    public interface IDriverService
    {
        Task<List<DriverListDto>> GetDriversAsync();
        Task<List<DriverHistoryDto>> GetDriverHistoryAsync(int driverId);
    }
}
