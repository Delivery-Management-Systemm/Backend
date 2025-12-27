namespace FMS.DAL.Interfaces
{
    //gom het tat ca repository vao 1 transaction duy nhat de tranh viec save changes nhieu lan
    public interface IUnitOfWork : IAsyncDisposable
    {

        IUserRepository Users { get; }
        IDriverRepository Drivers { get; }
        IVehicleRepository Vehicles { get; }
        ITripRepository Trips { get; }

        IDriverLicenseRepository DriverLicenses { get; }

        IExtraExpenseRepository ExtraExpenses { get; }
        IFuelRecordRepository FuelRecords { get; }
        ILicenseClassRepository LicenseClasses { get; }
        IMaintenanceRepository Maintenances { get; }
        ITripDriverRepository TripDrivers { get; }
        ITripLogRepository TripLogs { get; }
        IVehicleDriverAssignmentRepository VehicleDriverAssignments { get; }
        IMaintenanceServiceRepository MaintenanceServices { get; }
        IServiceRepository Services { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

