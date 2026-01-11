using FMS.DAL.Interfaces;
using FMS.Models;
using FMS.ServiceLayer.DTO.DriverDto;
using FMS.ServiceLayer.Interface;
using Microsoft.EntityFrameworkCore;

namespace FMS.ServiceLayer.Implementation
{
    public class DriverService: IDriverService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DriverService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DriverListDto>> GetDriversAsync()
        {
            var drivers = await _unitOfWork.Drivers.Query()
                .Include(d => d.DriverLicenses).ThenInclude(dl => dl.LicenseClass)
                .Include(d => d.TripDrivers)
                    .ThenInclude(td => td.Trip)
                    .ThenInclude(t => t.Vehicle)
                .Select(d => new DriverListDto
                {
                    DriverID = d.DriverID,
                    Name = d.FullName,
                    Phone = d.Phone,
                    ExperienceYears = d.ExperienceYears,

                    Licenses = d.DriverLicenses
                                .Select(l => l.LicenseClass.Code)
                                .Distinct()
                                .ToList(),

                    AssignedVehicle = d.TripDrivers
                        .Where(td => td.Trip.TripStatus == "In Progress") // Lấy chuyến đang chạy
                        .OrderByDescending(td => td.Trip.StartTime)
                        .Select(td => td.Trip.Vehicle.LicensePlate)
                        .FirstOrDefault() ?? "Đang rảnh",

                    TotalTrips = d.TotalTrips,
                    Rating = d.Rating,
                    Status = d.DriverStatus ?? "Active"
                })
                .ToListAsync();
            return drivers;

        }

        public async Task<List<DriverHistoryDto>> GetDriverHistoryAsync(int driverId)
        {
            if (driverId == null)
                throw new ArgumentException("Driver id not found");
            var history = await _unitOfWork.TripDrivers.Query()
                    .Where(td => td.DriverID == driverId)
                    .Include(td => td.Driver)
                    .Include(td => td.Trip)
                        .ThenInclude(t => t.Vehicle)
                    .Select(td => new DriverHistoryDto
                    {
                        DriverID = td.DriverID,
                        TripDate = td.Trip.StartTime,
                        DriverName = td.Driver.FullName,
                        VehiclePlate = td.Trip.Vehicle.LicensePlate,
                        Route = td.Trip.StartLocation + " - " + td.Trip.EndLocation,
                        DistanceKm = td.Trip.TotalDistanceKm,
                        DurationMinutes = td.Trip.EndTime != null
                            ? EF.Functions.DateDiffMinute(td.Trip.StartTime, td.Trip.EndTime)
                            : (int?)null,
                        TripRating = td.TripRating
                    })
                    .OrderByDescending(x => x.TripDate)
                    .ToListAsync();
            return history;
        }

        public async Task<DriverDetailsDto> GetDriverDetailsAsync(int driverId)
        {
            if (driverId == null)
                throw new ArgumentException("Driver id not found");
            var createdDriver = await _unitOfWork.Drivers
                .Query()
                .Include(d => d.DriverLicenses)
                    .ThenInclude(dl => dl.LicenseClass)
                .FirstAsync(d => d.DriverID == driverId);

            // ===== MAP RESPONSE =====
            return new DriverDetailsDto
            {
                DriverID = createdDriver.DriverID,
                FullName = createdDriver.FullName,
                Phone = createdDriver.Phone,
                Email = createdDriver.Email,
                BirthPlace = createdDriver.BirthPlace,
                ExperienceYears = createdDriver.ExperienceYears,
                TotalTrips = createdDriver.TotalTrips,
                Rating = createdDriver.Rating,
                DriverStatus = createdDriver.DriverStatus,

                Licenses = createdDriver.DriverLicenses.Select(dl => new DriverLicenseDto
                {
                    LicenseClassID = dl.LicenseClassID,
                    LicenseClassName = dl.LicenseClass.Code,
                    ExpiryDate = dl.ExpiryDate
                }).ToList()
            };
        }


        public async Task<DriverDetailsDto> CreateDriverAsync(CreateDriverDto dto)
        {
            // ===== VALIDATION =====
            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new Exception("Driver full name is required");

            if (dto.ExperienceYears < 0)
                throw new Exception("Experience years cannot be negative");

            if (dto.Licenses == null || !dto.Licenses.Any())
                throw new Exception("Driver must have at least one license");

            // ===== CREATE DRIVER =====
            var driver = new Driver
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email,
                BirthPlace = dto.BirthPlace,
                ExperienceYears = dto.ExperienceYears,

                TotalTrips = 0,
                Rating = null,
                DriverStatus = "Sẵn sàng"
            };

            await _unitOfWork.Drivers.AddAsync(driver);
            await _unitOfWork.SaveChangesAsync(); // lấy DriverID

            // ===== CREATE LICENSES =====
            var licenses = dto.Licenses.Select(l => new DriverLicense
            {
                DriverID = driver.DriverID,
                LicenseClassID = l.LicenseClassID,
                ExpiryDate = l.ExpiryDate
            }).ToList();

            await _unitOfWork.DriverLicenses.AddRangeAsync(licenses);
            await _unitOfWork.SaveChangesAsync();

            // ===== LOAD FULL DATA =====
            var createdDriver = await _unitOfWork.Drivers
                .Query()
                .Include(d => d.DriverLicenses)
                    .ThenInclude(dl => dl.LicenseClass)
                .FirstAsync(d => d.DriverID == driver.DriverID);

            // ===== MAP RESPONSE =====
            return new DriverDetailsDto
            {
                DriverID = createdDriver.DriverID,
                FullName = createdDriver.FullName,
                Phone = createdDriver.Phone,
                Email = createdDriver.Email,
                BirthPlace = createdDriver.BirthPlace,
                ExperienceYears = createdDriver.ExperienceYears,
                TotalTrips = createdDriver.TotalTrips,
                Rating = createdDriver.Rating,
                DriverStatus = createdDriver.DriverStatus,

                Licenses = createdDriver.DriverLicenses.Select(dl => new DriverLicenseDto
                {
                    LicenseClassID = dl.LicenseClassID,
                    LicenseClassName = dl.LicenseClass.Code,
                    ExpiryDate = dl.ExpiryDate
                }).ToList()
            };
        }
    }

}
