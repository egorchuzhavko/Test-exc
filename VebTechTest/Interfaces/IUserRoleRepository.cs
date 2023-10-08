using VebTechTest.Models;

namespace VebTechTest.Interfaces {
    public interface IUserRoleRepository {
        ICollection<UserRole> GetUsersRoles();
        bool UserRoleExists(int userid, int roleid);
        bool AddRole(UserRole userrole);
        bool Save();
    }
}
