using FMS.DAL.Interfaces;
using FMS.Models;
using FMS.ServiceLayer.DTO.VehicleDto;
using FMS.ServiceLayer.Interface;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace FMS.ServiceLayer.Implementation
{
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public VehicleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int SuggestLicenseClass(string vehicleType, string capacity)
        {
            vehicleType = vehicleType?.Trim().ToLowerInvariant() ?? "";
            capacity = capacity?.Trim().ToLowerInvariant() ?? "";
            // Parse capacity
            int ton = 0, seats = 0;
            if (capacity.Contains("tấn"))
            {
                var match = Regex.Match(capacity, @"(\d+)");
                if (match.Success)
                    ton = int.Parse(match.Groups[1].Value);
            }
            else if (capacity.Contains("chỗ"))
            {
                var match = Regex.Match(capacity, @"(\d+)");
                if (match.Success)
                    seats = int.Parse(match.Groups[1].Value);
            }
            System.Console.WriteLine(ton);System.Console.WriteLine(seats);
            System.Console.WriteLine(vehicleType);
            // Mapping rules
            if (vehicleType.Contains("xe con") || vehicleType.Contains("bán tải"))
                return 7; // B
            if (vehicleType.Contains("xe tải nhỏ") && ton >= 3.5 && ton <= 7.5)
                return 8; // C1
            if (vehicleType.Contains("xe tải lớn") && ton > 7.5)
                return 9; // C
            if (vehicleType.Contains("container"))
                return 15; // CE
            if (vehicleType.Contains("xe khách"))
            {
                if (seats <= 16)
                    return 10; // D1
                if (seats <= 29)
                    return 11; // D2
                if (seats >= 30)
                    return 12; // D
            }
            // Default fallback
            return 7; // B
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
                                VehicleBrand = v.VehicleBrand,
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
                Brand = vehicle.VehicleBrand ?? "N/A",
                Status = vehicle.VehicleStatus ?? "Sẵn sàng",
                Type = vehicle.VehicleType ?? "N/A",
                Year = vehicle.ManufacturedYear ?? 0,
                Mileage = vehicle.CurrentKm,
                Capacity = vehicle.Capacity ?? "N/A",

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

        public async Task<Vehicle> CreateVehicleAsync(VehicleCreateDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var normalizedPlate = dto.LicensePlate.Trim().ToUpperInvariant();
            var exists = await _unitOfWork.Vehicles.Query().AnyAsync(v => v.LicensePlate.ToUpper() == normalizedPlate);
            if (exists)
                throw new InvalidOperationException("Biển số xe đã tồn tại trong hệ thống.");

            int requiredLicenseClassId = SuggestLicenseClass(dto.VehicleType, dto.Capacity);
            var vehicle = new Vehicle
            {
                LicensePlate = normalizedPlate,
                VehicleType = dto.VehicleType.Trim(),
                VehicleBrand = dto.VehicleBrand.Trim(),
                VehicleModel = dto.VehicleModel.Trim(),
                ManufacturedYear = dto.ManufacturedYear,
                Capacity = dto.Capacity.Trim(),
                CurrentKm = dto.CurrentKm,
                FuelType = dto.FuelType,
                VehicleStatus = dto.VehicleStatus,
                RequiredLicenseClassID = requiredLicenseClassId
            };
            await _unitOfWork.Vehicles.AddAsync(vehicle);
            await _unitOfWork.SaveChangesAsync();
            return vehicle;
        }
    }
}
