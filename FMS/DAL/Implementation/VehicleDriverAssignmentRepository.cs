using FMS.DAL.Interfaces;
using FMS.Models;

namespace FMS.DAL.Implementation
{
    public class VehicleDriverAssignmentRepository: GenericRepository<VehicleDriverAssignment>, IVehicleDriverAssignmentRepository
    {
        public VehicleDriverAssignmentRepository(FMSDbContext context) : base(context)
        {
        }
    }
    
}
