using FMS.DAL.Interfaces;
using FMS.Models;
using FMS.Pagination;
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

        public async Task<PaginatedResult<MaintenanceListDto>> GetAllInvoiceAsync(MaintenanceParams @params)
        {
            var query = _unitOfWork.Maintenances.Query()
                .Include(m => m.Vehicle)
                .Include(m => m.MaintenanceServices)
                    .ThenInclude(ms => ms.Service)
                .AsNoTracking();


            if (!string.IsNullOrEmpty(@params.MaintenanceStatus))
            {
                query = query.Where(e => e.MaintenanceStatus == @params.MaintenanceStatus);
            }

            // Lọc theo Level (ví dụ: "high")
            if (!string.IsNullOrEmpty(@params.MaintenanceType))
            {
                query = query.Where(e => e.MaintenanceType == @params.MaintenanceType);
            }

            // 1. Xử lý Dynamic Sorting
            if (!string.IsNullOrEmpty(@params.SortBy))
            {
                query = @params.SortBy.ToLower() switch
                {
                    "maintenancestatus" => @params.IsDescending ? query.OrderByDescending(e => e.MaintenanceStatus) : query.OrderBy(e => e.MaintenanceStatus),
                    "totalcost" => @params.IsDescending ? query.OrderByDescending(e => e.TotalCost) : query.OrderBy(e => e.TotalCost),
                    "date" => @params.IsDescending ? query.OrderByDescending(e => e.ScheduledDate) : query.OrderBy(e => e.ScheduledDate),
                    // Mặc định sort theo ReportedAt như code cũ của bạn
                    _ => @params.IsDescending ? query.OrderByDescending(e => e.MaintenanceType) : query.OrderBy(e => e.MaintenanceType)
                };
            }
            var dtoQuery = query.Select(m => new MaintenanceListDto
            {
                Id = m.MaintenanceID.ToString(),
                // EF Core hỗ trợ string interpolation trong Select để dịch sang SQL
                InvoiceNumber = "HD-BT-" + m.MaintenanceID.ToString().PadLeft(4, '0'),
                VehicleId = m.VehicleID.ToString(),
                PlateNumber = m.Vehicle != null ? m.Vehicle.LicensePlate : "N/A",
                Date = m.ScheduledDate,
                Type = m.MaintenanceType,
                Workshop = m.GarageName,
                Technician = m.TechnicianName,
                TotalAmount = m.TotalCost,
                Notes = m.Notes,
                Status = m.MaintenanceStatus ?? "Unknown",
                // Map List con (Nested Collection)
                Services = m.MaintenanceServices.Select(ms => new MaintenanceServiceItemDto
                {
                    ServiceName = ms.Service.ServiceName,
                    Quantity = ms.Quantity,
                    Price = ms.UnitPrice,
                    Total = ms.TotalPrice
                }).ToList()
            });

            // 4. Gọi hàm paginate thần thánh của bạn
            // Kết quả trả về sẽ bao gồm total, limit, page và list objects
            return await dtoQuery.paginate(@params.PageSize, @params.PageNumber);
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
