using FMS.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using UITour.DAL.Interfaces;
using UITour.DAL.Interfaces.Repositories;
using UITour.DAL.Repositories;
using UITour.Models;

namespace UITour.DAL
{
    //tao db context 1 lan duy nhat
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FMSDbContext _context;

        public UnitOfWork(FMSDbContext context)
        {
            _context = context;
          
            Users = new UserRepository(_context);
           
        }

    
        public IUserRepository Users { get; }
      

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


