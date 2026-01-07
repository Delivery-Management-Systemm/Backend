using FMS.DAL.Interfaces;
using FMS.ServiceLayer.DTO.VehicleDto;
using FMS.ServiceLayer.Interface;
using Microsoft.EntityFrameworkCore;

namespace FMS.ServiceLayer.Implementation
{
    public class VehicleService: IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public VehicleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<VehicleListDto>> GetVehiclesAsync()
        {
            var vehicles = await _unitOfWork.Vehicles.Query()
                            .Include(v => v.RequiredLicenseClass)
                            .Select(v => new VehicleListDto
                            {
                                VehicleID = v.VehicleID,
                                LicensePlate = v.LicensePlate,
                                VehicleType = v.VehicleType,
                                VehicleModel = v.VehicleModel,
                                CurrentKm = v.CurrentKm,
                                VehicleStatus = v.VehicleStatus,
                                ManufacturedYear = v.ManufacturedYear,

                                RequiredLicenseClassID = v.RequiredLicenseClassID,
                                RequiredLicenseCode = v.RequiredLicenseClass.Code,
                                RequiredLicenseName = v.RequiredLicenseClass.LicenseDescription
                            })
                            .ToListAsync();
            return vehicles;
        }

        public async Task<VehicleDetailDto?> GetVehicleDetailsAsync(int vehicleId)
        {
            var directTrips = await _unitOfWork.Trips.Query() // Giả sử bạn có repository Trips
            .Where(t => t.VehicleID == vehicleId)
            .ToListAsync();

            Console.WriteLine($"Số lượng chuyến đi tìm thấy trực tiếp: {directTrips.Count}");
            var vehicle = await _unitOfWork.Vehicles.Query()
                .Include(v => v.Driver)
                .Include(v => v.Trips)
                    .ThenInclude(t => t.TripDrivers)
                    .ThenInclude(t => t.Driver) // Để lấy tên tài xế cho từng chuyến đi
                .Include(v => v.Maintenances)
                .FirstOrDefaultAsync(v => v.VehicleID == vehicleId);

            if (vehicle == null) return null;

            return new VehicleDetailDto
            {
                Id = vehicle.VehicleID.ToString(),
                PlateNumber = vehicle.LicensePlate,
                Model = vehicle.VehicleModel ?? "N/A",
                Brand = "Tải", // Nếu DB không có Brand riêng, bạn có thể mặc định hoặc parse từ Model
                Status = vehicle.VehicleStatus ?? "Sẵn sàng",
                Type = vehicle.VehicleType ?? "N/A",
                Year = vehicle.ManufacturedYear ?? 0,
                Mileage = vehicle.CurrentKm,

                AssignedDriverId = vehicle.DriverID?.ToString(),
                AssignedDriverName = vehicle.Driver?.FullName,

                // Map Lịch sử chuyến đi
                Trips = vehicle.Trips?.Select(t => new VehicleTripDto
                {
                    Id = t.TripID,
                    StartLocation = t.StartLocation,
                    EndLocation = t.EndLocation,
                    Status = t.TripStatus, // Hoàn thành, Đang thực hiện...
                    StartDate = t.StartTime,
                    Distance = t.TotalDistanceKm,
                    DriverName = t.TripDrivers
                            .Select(td => td.Driver?.FullName) // Chọn ra danh sách các FullName
                            .FirstOrDefault() ?? "N/A"         // Lấy cái đầu tiên, nếu không có thì trả về N/A
                }).ToList() ?? new(),

                // Map Lịch sử bảo trì
                Maintenances = vehicle.Maintenances?.Select(m => new VehicleMaintenanceDto
                {
                    Id = m.MaintenanceID,
                    Type = "Bảo trì", // Hoặc map dựa trên logic của bạn
                    Description = $"Bảo trì tại {m.GarageName}",
                    Status = m.MaintenanceStatus ?? "Hoàn thành",
                    Date = m.FinishedDate ?? m.ScheduledDate,
                    Cost = m.TotalCost,
                    Notes = m.GarageName
                }).ToList() ?? new()
            };
        }
    }
}
