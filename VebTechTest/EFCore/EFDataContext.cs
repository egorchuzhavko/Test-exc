using Microsoft.EntityFrameworkCore;
using VebTechTest.Models;

namespace VebTechTest.EFCore {
    public class EFDataContext : DbContext {
        public EFDataContext(DbContextOptions<EFDataContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<UserRole>()
                .HasOne(r => r.Role)
                .WithMany(ur => ur.UserRoles)
                .HasForeignKey(u => u.UserId);
            modelBuilder.Entity<UserRole>()
                .HasOne(u => u.User)
                .WithMany(ur => ur.UserRoles)
                .HasForeignKey(r => r.RoleId);
        }
    }
}
