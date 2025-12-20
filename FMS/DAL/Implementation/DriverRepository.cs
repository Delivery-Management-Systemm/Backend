using FMS.DAL.Interfaces.Repositories;
using FMS.DAL.Repositories;
using FMS.Models;


namespace FMS.DAL.Repositories
{
    public class DriverRepository : GenericRepository<Driver>, IDriverRepository
    {
        public DriverRepository(FMSDbContext context) : base(context)
        {
        }
    }
}


