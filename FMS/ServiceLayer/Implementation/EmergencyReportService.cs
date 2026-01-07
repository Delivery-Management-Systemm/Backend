using FMS.DAL.Interfaces;
using FMS.Models;
using FMS.ServiceLayer.DTO.EmergencyReportDto;
using FMS.ServiceLayer.Interface;
using Microsoft.EntityFrameworkCore;

namespace FMS.ServiceLayer.Implementation
{
    public class EmergencyReportService: IEmergencyReportService
    {
        private readonly IUnitOfWork _unitOfWork;
      
        public EmergencyReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
           
        }
        public async Task<List<EmergencyReportListDto>> GetAllAsync()
        {
            var emergencyReports = await _unitOfWork.EmergencyReports.Query()
                .Include(e => e.Vehicle)
                .Include(e => e.Driver)
                .OrderByDescending(e => e.ReportedAt)
                .Select(e => new EmergencyReportListDto
                {
                    Id = e.EmergencyID,
                    Title = e.Title,
                    Level = e.Level,
                    Status = e.Status,

                    Desc = e.Description,
                    Location = e.Location,
                    Contact = e.ContactPhone,

                    Reporter = e.Driver != null ? e.Driver.FullName : "Không xác định",
                    Driver = e.Driver != null ? e.Driver.FullName : "-",

                    Vehicle = e.Vehicle != null
                        ? e.Vehicle.LicensePlate + " - " + e.Vehicle.VehicleType
                        : "-",

                    ReportedAt = e.ReportedAt.ToString("HH:mm:ss dd/MM/yyyy"),
                    RespondedAt = e.RespondedAt != null
                        ? e.RespondedAt.Value.ToString("HH:mm:ss dd/MM/yyyy")
                        : null,
                    ResolvedAt = e.ResolvedAt != null
                        ? e.ResolvedAt.Value.ToString("HH:mm:ss dd/MM/yyyy")
                        : null
                })
                .ToListAsync();
            return emergencyReports;
        }
    }
}
