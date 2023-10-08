using VebTechTest.EFCore;
using VebTechTest.Interfaces;
using VebTechTest.Models;

namespace VebTechTest.Repository {
    public class RoleRepository : IRoleRepository {
        private readonly EFDataContext _context;

        public RoleRepository(EFDataContext context) {
            this._context = context;
        }

        public bool RoleExists(int id) {
            return _context.Roles.Any(r => r.Id == id);
        }
    }
}