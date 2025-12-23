using FMS.DAL.Interfaces;
using FMS.Models;
using Microsoft.EntityFrameworkCore;



namespace FMS.DAL.Implementation
{
    //tao db context 1 lan duy nhat
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FMSDbContext _context;

        public UnitOfWork(FMSDbContext context)
        {
            _context = context;

            Users = new UserRepository(_context);
            Drivers = new DriverRepository(_context);
            Vehicles = new VehicleRepository(_context);
            Trips = new TripRepository(_context);
            DriverLicenses = new DriverLicenseRepository(_context);
            ExtraExpenses = new ExtraExpenseRepository(_context);
            FuelRecords = new FuelRecordRepository(_context);
            LicenseClasses = new LicenseClassRepository(_context);
            Maintenances = new MaintenanceRepository(_context);
        }


        public IUserRepository Users { get; }
        public IDriverRepository Drivers { get; }
        public IVehicleRepository Vehicles { get; }
        public ITripRepository Trips { get; }
        public IDriverLicenseRepository DriverLicenses { get; }
        public IExtraExpenseRepository ExtraExpenses { get; }
        public IFuelRecordRepository FuelRecords { get; }
        public ILicenseClassRepository LicenseClasses { get; }
        public IMaintenanceRepository Maintenances { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}


