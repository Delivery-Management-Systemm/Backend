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
                    ExperienceYears = d.ExperienceYears,

                    Licenses = d.DriverLicenses
                                .Select(l => l.LicenseClass.Code)
                                .Distinct()
                                .ToList(),

                    AssignedVehicle = d.VehicleAssignments
                                .Where(a=>a.AssignedTo == null)
                                .OrderByDescending(a => a.AssignedFrom)
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
