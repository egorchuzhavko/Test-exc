using VebTechTest.EFCore;
using VebTechTest.Interfaces;
using VebTechTest.Models;

namespace VebTechTest.Repository {
    public class UserRoleRepository : IUserRoleRepository {
        private readonly EFDataContext _context;

        public ICollection<UserRole> GetUsersRoles() {
            return _context.UserRoles.ToList();
        }

        public UserRoleRepository(EFDataContext context) {
            this._context = context;
        }

        public bool UserRoleExists(int userid, int roleid) {
            return _context.UserRoles.Any(ur => ur.RoleId == roleid & ur.UserId == userid);
        }

        public bool AddRole(UserRole userrole) {
            _context.Add(userrole);
            return Save();
        }

        public bool Save() {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

    }
}