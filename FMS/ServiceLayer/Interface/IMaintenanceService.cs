using FMS.Models;
using FMS.ServiceLayer.DTO.MaintenanceDto;

namespace FMS.ServiceLayer.Interface
{
    public interface IMaintenanceService
    {
        Task<List<ServiceDto>> GetAllServiceAsync();
        Task<List<MaintenanceListDto>> GetAllInvoiceAsync();
    }
}
