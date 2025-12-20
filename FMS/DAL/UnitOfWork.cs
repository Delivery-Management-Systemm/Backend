using FMS.DAL.Interfaces;
using FMS.DAL.Interfaces.Repositories;
using FMS.DAL.Repositories;
using FMS.Models;
using Microsoft.EntityFrameworkCore;



namespace FMS.DAL
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
        }

    
        public IUserRepository Users { get; }
        public IDriverRepository Drivers { get; }
        public IVehicleRepository Vehicles { get; }

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


