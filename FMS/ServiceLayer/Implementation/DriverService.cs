using FMS.DAL.Interfaces;
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
                .Include(d => d.VehicleAssignments).ThenInclude(va => va.Vehicle)
                .Select(d => new DriverListDto
                {
                    DriverID = d.DriverID,
                    Name = d.FullName,
                    Phone = d.Phone,

                    LicenseNumber = d.DriverLicenses
                    .OrderByDescending(l => l.ExpiryDate)
                    .Select(l => l.LicenseClass.Code)
                    .FirstOrDefault() ?? "N/A",

                    LicenseExpiry = d.DriverLicenses
                    .OrderByDescending(l => l.ExpiryDate)
                    .Select(l => l.ExpiryDate)
                    .FirstOrDefault(),

                    AssignedVehicle = d.VehicleAssignments
                    .Select(a => a.Vehicle.LicensePlate)
                    .FirstOrDefault() ?? "Chưa gán",

                    TotalTrips = d.TotalTrips,
                    Rating = d.Rating,
                    Status = d.DriverStatus ?? "Active"
                })
                .ToListAsync();
            return drivers;

        }
    }

}
