using FMS.DAL.Interfaces;
using FMS.ServiceLayer.DTO.TripDto;
using FMS.ServiceLayer.Interface;
using Microsoft.EntityFrameworkCore;

namespace FMS.ServiceLayer.Implementation
{
    public class TripService : ITripService
    {
        private readonly IUnitOfWork _unitOfWork;
        public TripService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<TripListDto>> GetTripsAsync()
        {
            var trips = await _unitOfWork.Trips
               .Query()
               .Include(t => t.Vehicle)
               .Include(t => t.TripDrivers)
                   .ThenInclude(td => td.Driver)
               .Include(t => t.ExtraExpenses)
               .OrderByDescending(t => t.StartTime)
               .Select(t => new TripListDto
               {
                   Id = t.TripID,

                   Vehicle = t.Vehicle.LicensePlate,

                   Driver = t.TripDrivers
                       .Where(td => td.Role == "Main Driver")
                       .Select(td => td.Driver.FullName)
                       .FirstOrDefault() ?? "Chưa gán",

                   Route = t.StartLocation + " - " + t.EndLocation,

                   Date = t.StartTime.ToString("dd/MM/yyyy"),

                   Time = t.EndTime != null
                       ? $"{t.StartTime:HH:mm} - {t.EndTime:HH:mm}"
                       : null,

                   Distance = t.TotalDistanceKm != null
                       ? t.TotalDistanceKm + " km"
                       : "0 km",

                   Cost = t.ExtraExpenses.Any()
                       ? string.Format("{0:N0}đ", t.ExtraExpenses.Sum(e => e.Amount))
                       : "0đ",

                   Status = t.TripStatus == "Completed"
                       ? "completed"
                       : "in-progress",

                   Multiday = t.EndTime != null &&
                              t.StartTime.Date != t.EndTime.Value.Date,

                   Charges = t.ExtraExpenses.Select(e => new TripChargeDto
                   {
                       Id = e.ExtraExpenseID,
                       Name = e.ExpenseType,
                       AmountNumber = e.Amount,
                       //Amount = string.Format("{0:N0}đ", e.Amount)
                   }).ToList()
               })
               .ToListAsync();

            return trips;
        }
        public async Task<TripStatsDto> GetTripStatsAsync()
        {
            var today = DateTime.Today;

            var query = _unitOfWork.Trips.Query();

            return new TripStatsDto
            {
                TodayTrips = await query.CountAsync(t => t.StartTime.Date == today),
                InProgress = await query.CountAsync(t => t.TripStatus == "In Progress"),
                Completed = await query.CountAsync(t => t.TripStatus == "Completed"),
                TotalDistance = string.Format("{0:N0} km",
                    await query.SumAsync(t => t.TotalDistanceKm ?? 0))
            };
        }

        public async Task<List<OrderListDto>> GetOrdersAsync()
        {
            var trips = await _unitOfWork.Trips.Query()
                                        .Include(t => t.Vehicle)
                                        .Include(t => t.TripDrivers)
                                            .ThenInclude(td => td.Driver)
                                        .Include(t => t.TripSteps)
                                        .Include(t => t.ExtraExpenses)
                                        .ToListAsync();

            var result = trips.Select(t => new OrderListDto
            {
                Id = t.TripID,
                Customer = t.CustomerName,
                Contact = t.CustomerPhone,
                Pickup = t.StartLocation,
                Dropoff = t.EndLocation,
                Vehicle = t.Vehicle.LicensePlate,
                Driver = t.TripDrivers
                            .OrderByDescending(td => td.AssignedFrom)
                            .Select(td => td.Driver.FullName)
                            .FirstOrDefault(),

                Status = t.TripStatus,

                Steps = t.TripSteps
                    .OrderBy(s => s.TripStepID)
                    .Select(s => new TripStepDto
                    {
                        Key = s.StepKey,
                        Label = s.StepLabel,
                        Done = s.IsDone,
                        Time = s.ConfirmedAt != null
                            ? s.ConfirmedAt.Value.ToString("HH:mm:ss dd/MM/yyyy")
                            : null
                    }).ToList(),

                Cost = $"{t.ExtraExpenses.Sum(e => e.Amount):N0}đ"
            }).ToList();
            return result;
        }
    }
}
