using Microsoft.EntityFrameworkCore;
using VebTechTest.Models;

namespace VebTechTest.EFCore {
    public class EFDataContext : DbContext {
        public EFDataContext(DbContextOptions<EFDataContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
