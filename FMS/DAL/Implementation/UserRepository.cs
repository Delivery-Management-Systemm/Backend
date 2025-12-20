using FMS.DAL.Interfaces.Repositories;
using FMS.Models;

namespace FMS.DAL.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(FMSDbContext context) : base(context)
        {
        }
    }
}


