using FMS.DAL.Interfaces;
using FMS.Models;
using FMS.ServiceLayer.Interface;
using Microsoft.EntityFrameworkCore;

namespace FMS.ServiceLayer.Implementation
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        public MaintenanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<Service>> GetAllServiceAsync()
        {
            var services = await _unitOfWork.Services.Query().ToListAsync();
            return services;
        }
    }
}
