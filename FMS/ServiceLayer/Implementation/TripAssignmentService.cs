using FMS.DAL.Interfaces;
using FMS.Models;
using FMS.ServiceLayer.Interface;
using Microsoft.EntityFrameworkCore;

namespace FMS.ServiceLayer.Implementation
{
    public class TripAssignmentService : ITripAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TripAssignmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AssignVehicleAndDriverAsync(int tripId)
        {
            // 1. Lấy trip
            var trip = await _unitOfWork.Trips.Query()
                .Include(t => t.TripDrivers)
                .FirstOrDefaultAsync(t => t.TripID == tripId);

            if (trip == null)
                throw new Exception("Trip not found");

            if (trip.TripStatus != "Planned")
                throw new Exception("Trip is not in Planned status");

            // 2. Chọn VEHICLE phù hợp
            var vehicle = await _unitOfWork.Vehicles.Query()
                .Include(v => v.RequiredLicenseClass)
                .Where(v =>
                    v.VehicleStatus == "available" //&&
                    //v.VehicleType == trip.RequestedVehicleType
                )
                //.OrderBy(v => v.CurrentKm) // ưu tiên xe chạy ít
                .FirstOrDefaultAsync();

            if (vehicle == null)
                throw new Exception("No suitable vehicle found");

            // 3. Chọn DRIVER phù hợp với xe
            var driver = await _unitOfWork.Drivers.Query()
                .Include(d => d.DriverLicenses)
                    .ThenInclude(dl => dl.LicenseClass)
                .Where(d =>
                    d.DriverStatus == "available" &&
                    d.DriverLicenses.Any(dl =>
                        dl.ExpiryDate > DateTime.Now &&
                        dl.LicenseClass.Rank >= vehicle.RequiredLicenseClass.Rank
                    )
                )
                //.OrderBy(d => d.TotalTrips) // ưu tiên driver ít trip
                .FirstOrDefaultAsync();

            if (driver == null)
                throw new Exception("No suitable driver found");

            // 4. GÁN VEHICLE & DRIVER
            trip.VehicleID = vehicle.VehicleID;
            trip.TripStatus = "confirmed";

            var tripDriver = new TripDriver
            {
                TripID = trip.TripID,
                DriverID = driver.DriverID,
                Role = "Main Driver",
                AssignedFrom = DateTime.Now
            };

            await _unitOfWork.TripDrivers.AddAsync(tripDriver);

            // 5. UPDATE trạng thái
            vehicle.VehicleStatus = "in_use";
            driver.DriverStatus = "on_trip";

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
