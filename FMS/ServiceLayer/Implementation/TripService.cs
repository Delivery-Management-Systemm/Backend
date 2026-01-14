using FMS.DAL.Interfaces;
using FMS.Models;
using FMS.Pagination;
using FMS.ServiceLayer.DTO.TripDto;
using FMS.ServiceLayer.Interface;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FMS.ServiceLayer.Implementation
{
    public class TripService : ITripService
    {
        private readonly IUnitOfWork _unitOfWork;
        public TripService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PaginatedResult<TripListDto>> GetTripsAsync(TripParams @params)
        {
            // 1. Khởi tạo query (Bỏ Include vì Select sẽ tự động JOIN)
            var query = _unitOfWork.Trips.Query().AsNoTracking();

            // --- BƯỚC 1: LỌC (FILTERING) ---
            if (!string.IsNullOrEmpty(@params.TripStatus))
            {
                query = query.Where(t => t.TripStatus == @params.TripStatus);
            }

            // --- BƯỚC 2: SẮP XẾP (SORTING) ---
            // Lưu ý: Sửa lại các case cho khớp với ToLower()
            if (!string.IsNullOrEmpty(@params.SortBy))
            {
                query = @params.SortBy.ToLower() switch
                {
                    "status" => @params.IsDescending ? query.OrderByDescending(t => t.TripStatus) : query.OrderBy(t => t.TripStatus),
                    "distance" => @params.IsDescending ? query.OrderByDescending(t => t.TotalDistanceKm) : query.OrderBy(t => t.TotalDistanceKm),
                    "starttime" => @params.IsDescending ? query.OrderByDescending(t => t.StartTime) : query.OrderBy(t => t.StartTime),
                    // Default sort theo StartTime nếu SortBy không khớp
                    _ => @params.IsDescending ? query.OrderByDescending(t => t.StartTime) : query.OrderBy(t => t.StartTime)
                };
            }
            else
            {
                query = query.OrderByDescending(t => t.StartTime);
            }

            // --- BƯỚC 3: MAPPING SANG DTO ---
            var dtoQuery = query.Select(t => new TripListDto
            {
                Id = t.TripID,
                Vehicle = t.Vehicle.LicensePlate,

                // Lấy tài xế chính
                Driver = t.TripDrivers
                    .Where(td => td.Role == "Main Driver")
                    .Select(td => td.Driver.FullName)
                    .FirstOrDefault() ?? "Chưa gán",

                Route = t.StartLocation + " - " + t.EndLocation,

                // Lưu ý: Một số bản EF Core cũ không dịch được .ToString("dd/MM/yyyy") sang SQL.
                // Nếu lỗi, hãy trả về DateTime thô và format ở Frontend.
                Date = t.StartTime.ToString("dd/MM/yyyy"),

                Time = t.EndTime != null

                ? $"{t.StartTime:HH:mm} - {t.EndTime:HH:mm}"

                : null,

                Distance = (t.TotalDistanceKm ?? 0) + " km",

                // Tính tổng chi phí (EF Core dịch Sum() sang SQL rất tốt)
                Cost = t.ExtraExpenses.Any()
                    ? t.ExtraExpenses.Sum(e => e.Amount).ToString() + "đ"
                    : "0đ",

                Status = t.TripStatus == "Completed" ? "completed" : "in-progress",

                Multiday = t.EndTime != null && t.StartTime.Date != t.EndTime.Value.Date,

                Charges = t.ExtraExpenses.Select(e => new TripChargeDto
                {
                    Id = e.ExtraExpenseID,
                    Name = e.ExpenseType,
                    AmountNumber = e.Amount
                }).ToList()
            });

            // --- BƯỚC 4: PHÂN TRANG ---
            // Sử dụng logic skip = page * limit của bạn
            return await dtoQuery.paginate(@params.PageSize, @params.PageNumber);
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

        public async Task<OrderListDto> GetOrdersByIdAsync(int tripId)
        {
            if (tripId == null)
                throw new ArgumentException("Trip id not found");
            var trip = await _unitOfWork.Trips.Query()
                                        .Include(t => t.Vehicle)
                                        .Include(t => t.TripDrivers)
                                            .ThenInclude(td => td.Driver)
                                        .Include(t => t.TripSteps)
                                        .Include(t => t.ExtraExpenses)
                                        .FirstAsync(d => d.TripID == tripId);

            return new OrderListDto
            {
                Id = trip.TripID,
                Customer = trip.CustomerName,
                Contact = trip.CustomerPhone,
                Pickup = trip.StartLocation,
                Dropoff = trip.EndLocation,
                Vehicle = trip.Vehicle.LicensePlate,
                Driver = trip.TripDrivers
                            .OrderByDescending(td => td.AssignedFrom)
                            .Select(td => td.Driver.FullName)
                            .FirstOrDefault(),

                Status = trip.TripStatus,

                Steps = trip.TripSteps
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

                Cost = $"{trip.ExtraExpenses.Sum(e => e.Amount):N0}đ"
            };
         
        }

        public async Task<PaginatedResult<BookedTripListDto>> GetBookedTripListAsync(BookedTripParams @params)
        {
            var query =  _unitOfWork.Trips.Query()
                .Where(t => t.ScheduledStartTime != null && t.TripStatus == "Planned").AsNoTracking();



            // --- BƯỚC 2: SẮP XẾP (SORTING) ---
            // Lưu ý: Sửa lại các case cho khớp với ToLower()
            if (!string.IsNullOrEmpty(@params.SortBy))
            {
                query = @params.SortBy.ToLower() switch
                {
                    "starttime" => @params.IsDescending ? query.OrderByDescending(t => t.StartTime) : query.OrderBy(t => t.StartTime),
                    // Default sort theo StartTime nếu SortBy không khớp
                    _ => @params.IsDescending ? query.OrderByDescending(t => t.StartTime) : query.OrderBy(t => t.StartTime)
                };
            }

            var dtoQuery = query.Select(t => new BookedTripListDto
                {
                    TripID = t.TripID,
                    // Customer
                    CustomerName = t.CustomerName,
                    CustomerPhone = t.CustomerPhone,
                    CustomerEmail = t.CustomerEmail,
                    // Route
                    PickupLocation = t.StartLocation,
                    DropoffLocation = t.EndLocation,
                    // Schedule
                    ScheduledDate = t.ScheduledStartTime.Value.Date,
                    ScheduledTime = t.ScheduledStartTime.Value.ToString("HH:mm"),
                    // Request
                    VehicleType = t.RequestedVehicleType,
                    Passengers = t.RequestedPassengers,
                    Cargo = t.RequestedCargo,
                    // Assignment
                    AssignedVehicleId = t.VehicleID,
                    AssignedVehiclePlate = t.Vehicle.LicensePlate,
                    AssignedDriverId = t.TripDrivers
                                        .Where(td => td.Role == "Main Driver")
                                        .Select(td => td.DriverID)
                                        .FirstOrDefault(),
                    AssignedDriverName = t.TripDrivers
                                        .Where(td => td.Role == "Main Driver")
                                        .Select(td => td.Driver.FullName)
                                        .FirstOrDefault(),
                    Status = t.TripStatus
                });
            return await dtoQuery.paginate(@params.PageSize, @params.PageNumber);
        }

        public async Task<BookedTripStatsDto> GetBookedTripStatsAsync()
        {
            var trips = _unitOfWork.Trips.Query();
            return new BookedTripStatsDto
            {
                Planned = await trips.CountAsync(t => t.TripStatus == "Planned"),
                Confirmed = await trips.CountAsync(t => t.TripStatus == "Confirmed")
            };

        }

        public async Task<Trip> CreateBookingTripAsync(CreateBookingTripDto dto)
        {
            var trip = new Trip
            {
                // CHƯA ASSIGN
                VehicleID = null,

                // CUSTOMER
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                CustomerEmail = dto.CustomerEmail,

                // ROUTE
                StartLocation = dto.StartLocation,
                EndLocation = dto.EndLocation,
                RouteGeometryJson = dto.RouteGeometryJson,

                // ESTIMATE
                EstimatedDistanceKm = dto.EstimatedDistanceKm,
                EstimatedDurationMin = dto.EstimatedDurationMin,

                // BOOKING TIME
                ScheduledStartTime = dto.ScheduledStartTime,

                // REQUEST
                RequestedVehicleType = dto.RequestedVehicleType,
                RequestedPassengers = dto.RequestedPassengers,
                RequestedCargo = dto.RequestedCargo,

                // STATUS
                TripStatus = "planned",
                StartTime = dto.ScheduledStartTime
            };

            await _unitOfWork.Trips.AddAsync(trip);
            await _unitOfWork.SaveChangesAsync();

            return trip;
        }

    }
}
