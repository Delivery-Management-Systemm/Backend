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
    }
}
