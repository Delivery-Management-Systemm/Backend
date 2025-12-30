using FMS.ServiceLayer.DTO.DriverDto;
using FMS.ServiceLayer.DTO.VehicleDto;

namespace FMS.ServiceLayer.Interface
{
    public interface IVehicleService
    {
        Task<List<VehicleListDto>> GetVehiclesAsync();
    }
}
