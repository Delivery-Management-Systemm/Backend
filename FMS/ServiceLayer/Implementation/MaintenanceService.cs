using FMS.DAL.Interfaces;
using FMS.Models;
using FMS.ServiceLayer.DTO.MaintenanceDto;
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
        public async Task<List<ServiceDto>> GetAllServiceAsync()
        {
            var services = await _unitOfWork.Services.Query()
                .Select(s => new ServiceDto
                {
                    Id = s.ServiceID,
                    Name = s.ServiceName,
                    Price = s.ServicePrice,
                    Category = s.ServiceType
                }).ToListAsync();
            return services;
        }

        public async Task<List<MaintenanceListDto>> GetAllInvoiceAsync()
        {
            var data = await _unitOfWork.Maintenances.Query()
                .Include(m => m.Vehicle)
                .Include(m => m.MaintenanceServices)
                    .ThenInclude(ms => ms.Service)
                .OrderByDescending(m => m.ScheduledDate)
                .ToListAsync();

            return data.Select(m => new MaintenanceListDto
            {
                Id = m.MaintenanceID.ToString(),
                InvoiceNumber = $"HD-BT-{m.MaintenanceID:D4}",
                VehicleId = m.VehicleID.ToString(),
                PlateNumber = m.Vehicle?.LicensePlate ?? "N/A",
                Date = m.FinishedDate ?? m.ScheduledDate,
                Type = m.MaintenanceStatus ?? "Sửa chữa", // Hoặc lấy từ Service đầu tiên
                Workshop = m.GarageName,
                Technician = m.TechnicianName,
                TotalAmount = m.TotalCost,
                Services = m.MaintenanceServices.Select(ms => new MaintenanceServiceItemDto
                {
                    ServiceName = ms.Service.ServiceName,
                    Quantity = ms.Quantity,
                    Price = ms.UnitPrice,
                    Total = ms.TotalPrice
                }).ToList()
            }).ToList();
        }
    }
}
