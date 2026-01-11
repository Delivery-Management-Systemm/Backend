using FMS.Models;
using FMS.ServiceLayer.DTO.DriverDto;
using FMS.ServiceLayer.DTO.VehicleDto;

namespace FMS.ServiceLayer.Interface
{
    public interface IVehicleService
    {
        Task<List<VehicleListDto>> GetVehiclesAsync();
        Task<VehicleDetailDto?> GetVehicleDetailsAsync(int vehicleId);
        Task<Vehicle> CreateVehicleAsync(VehicleCreateDto dto);
    }
}
