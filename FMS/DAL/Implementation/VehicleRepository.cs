using FMS.DAL.Interfaces.Repositories;
using FMS.DAL.Repositories;
using FMS.Models;


namespace FMS.DAL.Repositories
{
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(FMSDbContext context) : base(context)
        {
        }
    }
}


