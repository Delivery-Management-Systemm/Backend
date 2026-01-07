using FMS.Models;

namespace FMS.ServiceLayer.Interface
{
    public interface IMaintenanceService
    {
        Task<List<Service>> GetAllServiceAsync();
    }
}
