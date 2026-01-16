using FMS.DAL.Interfaces;
using FMS.Models;
using FMS.Pagination;
using FMS.ServiceLayer.DTO.DriverDto;
using FMS.ServiceLayer.DTO.EmergencyReportDto;
using FMS.ServiceLayer.Interface;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FMS.ServiceLayer.Implementation
{
    public class DriverService : IDriverService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DriverService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedResult<DriverListDto>> GetDriversAsync(DriverParams @params)
        {
            var query = _unitOfWork.Drivers.Query()
                .Include(d => d.DriverLicenses).ThenInclude(dl => dl.LicenseClass)
                .Include(d => d.TripDrivers)
                    .ThenInclude(td => td.Trip)
                    .ThenInclude(t => t.Vehicle)
                .Include(d => d.User)
                .AsNoTracking(); // Tăng hiệu năng cho query chỉ đọc


            if (!string.IsNullOrEmpty(@params.DriverStatus))
            {
                query = query.Where(e => e.DriverStatus == @params.DriverStatus);
            }



            // 1. Xử lý Dynamic Sorting
            if (!string.IsNullOrEmpty(@params.SortBy))
            {
                query = @params.SortBy.ToLower() switch
                {
                    "experience" => @params.IsDescending ? query.OrderByDescending(e => e.ExperienceYears) : query.OrderBy(e => e.ExperienceYears),
                    "status" => @params.IsDescending ? query.OrderByDescending(e => e.DriverStatus) : query.OrderBy(e => e.DriverStatus),
                    "rating" => @params.IsDescending ? query.OrderByDescending(e => e.Rating) : query.OrderBy(e => e.Rating),
                    _ => @params.IsDescending ? query.OrderByDescending(e => e.User.FullName) : query.OrderBy(e => e.User.FullName)
                };
            }
            else
            {
                query = query.OrderBy(e => e.User.FullName);
            }

            var dtoQuery = query.Select(d => new DriverListDto
                {
                    DriverID = d.DriverID,
                    Name = d.User.FullName,
                    Phone = d.User.Phone,
                    Email = d.User.Email,
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
                });
            return await dtoQuery.paginate(@params.PageSize, @params.PageNumber);

        }

        public async Task<List<DriverHistoryDto>> GetDriverHistoryAsync(int driverId)
        {
            if (driverId == null)
                throw new ArgumentException("Driver id not found");
            var history = await _unitOfWork.TripDrivers.Query()
                    .Where(td => td.DriverID == driverId)
                    .Include(td => td.Driver)
                        .ThenInclude(d => d.User)
                    .Include(td => td.Trip)
                        .ThenInclude(t => t.Vehicle)
                    .Select(td => new DriverHistoryDto
                    {
                        DriverID = td.DriverID,
                        TripDate = td.Trip.StartTime,
                        DriverName = td.Driver.User.FullName,
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
                .Include(d => d.User)
                .FirstAsync(d => d.DriverID == driverId);

            // ===== MAP RESPONSE =====
            return new DriverDetailsDto
            {
                DriverID = createdDriver.DriverID,
                FullName = createdDriver.User.FullName,
                Phone = createdDriver.User.Phone,
                Email = createdDriver.User.Email,
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


        /*public async Task<DriverDetailsDto> CreateDriverAsync(CreateDriverDto dto)
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
                FullName = createdDriver.User.FullName,
                Phone = createdDriver.User.Phone,
                Email = createdDriver.User.Email,
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
        }*/

        public async Task UpdateDriverRatingAsync(int driverId)
        {
            var driver = await _unitOfWork.Drivers.Query()
                .Include(d => d.TripDrivers)
                .ThenInclude(td => td.Trip)
                .FirstOrDefaultAsync(d => d.DriverID == driverId);
            if (driver == null)
                throw new Exception("Driver not found");

            var completedTrips = driver.TripDrivers
                .Where(td => td.Trip.TripStatus == "Completed" && td.TripRating.HasValue)
                .ToList();

            if (completedTrips.Count == 0)
            {
                driver.Rating = null;
            }
            else
            {
                driver.Rating = completedTrips.Average(td => td.TripRating.Value);
            }
            _unitOfWork.Drivers.Update(driver);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateDriverAsync(int driverId, UpdateDriverDto dto)
        {
            var driver = await _unitOfWork.Drivers.GetByIdAsync(driverId);
            if (driver == null) return false;

            if (!string.IsNullOrEmpty(dto.FullName))
                driver.User.FullName = dto.FullName.Trim();
            if (!string.IsNullOrEmpty(dto.Phone))
                driver.User.Phone = dto.Phone.Trim();
            if (!string.IsNullOrEmpty(dto.Email))
                driver.User.Email = dto.Email.Trim();
            if (!string.IsNullOrEmpty(dto.BirthPlace))
                driver.BirthPlace = dto.BirthPlace.Trim();
            if (dto.ExperienceYears.HasValue)
                driver.ExperienceYears = dto.ExperienceYears.Value;
            if (!string.IsNullOrEmpty(dto.DriverStatus))
                driver.DriverStatus = dto.DriverStatus;

            _unitOfWork.Drivers.Update(driver);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDriverAsync(int driverId)
        {
            var driver = await _unitOfWork.Drivers.GetByIdAsync(driverId);
            if (driver == null) return false;

            _unitOfWork.Drivers.Remove(driver);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
