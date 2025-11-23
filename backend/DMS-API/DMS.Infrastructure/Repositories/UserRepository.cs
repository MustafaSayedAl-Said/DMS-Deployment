using DMS.Core.Entities;
using DMS.Core.Interfaces;
using DMS.Infrastructure.Data;

namespace DMS.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        //public bool userExists(int id)
        //{
        //    return _context.Users.Any(u => u.Id == id);
        //}
    }
}
