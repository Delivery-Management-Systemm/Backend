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
                Date = m.ScheduledDate,
                Type = m.MaintenanceType, // Hoặc lấy từ Service đầu tiên
                Workshop = m.GarageName,
                Technician = m.TechnicianName,
                TotalAmount = m.TotalCost,
                Notes = m.Notes,
                Status = m.MaintenanceStatus ?? "Unknown",
                Services = m.MaintenanceServices.Select(ms => new MaintenanceServiceItemDto
                {
                    ServiceName = ms.Service.ServiceName,
                    Quantity = ms.Quantity,
                    Price = ms.UnitPrice,
                    Total = ms.TotalPrice
                }).ToList()
            }).ToList();
        }

        public async Task<int> CreateMaintenanceAsync(CreateMaintenanceDto dto)
        {
            if (dto.Services == null || !dto.Services.Any())
                throw new Exception("Maintenance must have at least one service");

            var maintenance = new Maintenance
            {
                VehicleID = dto.VehicleID,
                MaintenanceType = dto.MaintenanceType,
                ScheduledDate = dto.ScheduledDate,
                GarageName = dto.GarageName,
                TechnicianName = dto.TechnicianName,
                Notes = dto.Notes,
                NextMaintenanceDate = dto.NextMaintenanceDate,
                NextMaintenanceKm = dto.NextMaintenanceKm,
                MaintenanceStatus = dto.MaintenanceStatus ?? "scheduled",
                MaintenanceServices = new List<FMS.Models.MaintenanceService>()
            };

            double totalCost = 0;

            foreach (var s in dto.Services)
            {
                var service = await _unitOfWork.Services
                    .GetByIdAsync(s.ServiceID);

                if (service == null)
                    throw new Exception($"Service {s.ServiceID} not found");

                var quantity = s.Quantity <= 0 ? 1 : s.Quantity;

                var unitPrice = s.UnitPrice ?? service.ServicePrice;

                var maintenanceService = new FMS.Models.MaintenanceService
                {
                    ServiceID = service.ServiceID,
                    Quantity = quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = unitPrice * quantity
                };

                totalCost += maintenanceService.TotalPrice;
                maintenance.MaintenanceServices.Add(maintenanceService);
            }

            maintenance.TotalCost = totalCost;

            await _unitOfWork.Maintenances.AddAsync(maintenance);
            await _unitOfWork.SaveChangesAsync();

            return maintenance.MaintenanceID;
        }


    }
}
