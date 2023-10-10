using VebTechTest.EFCore;
using VebTechTest.Models;

namespace VebTechTest {
    public class RoleFiller {
        private readonly EFDataContext dataContext;
        public RoleFiller(EFDataContext context) {
            this.dataContext = context;
        }
        public void FillRoles() {
            if(!dataContext.Roles.Any()) {
                var listroles = new List<Role>() {
                    new Role(){ Type="User" },
                    new Role(){ Type="Admin" },
                    new Role(){ Type="Support" },
                    new Role(){ Type="SuperAdmin" }
                };
                dataContext.Roles.AddRange(listroles);
                dataContext.SaveChanges();
            }
        }
    }
}
