using FMS.Pagination;
using FMS.ServiceLayer.DTO.DriverDto;

namespace FMS.ServiceLayer.Interface
{
    public interface IDriverService
    {
        Task<PaginatedResult<DriverListDto>> GetDriversAsync(DriverParams @params);
        Task<List<DriverHistoryDto>> GetDriverHistoryAsync(int driverId);
        Task<DriverDetailsDto> GetDriverDetailsAsync(int driverId);
        Task<DriverDetailsDto> CreateDriverAsync(CreateDriverDto dto);
        Task UpdateDriverRatingAsync(int driverId);
    }
}
