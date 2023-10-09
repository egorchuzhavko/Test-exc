using VebTechTest.EFCore;
using VebTechTest.Models;
using VebTechTest.Interfaces;
using Microsoft.EntityFrameworkCore;
using VebTechTest.DTO;

namespace VebTechTest.Repository {
    public class UserRepository : IUserRepository {
        private readonly EFDataContext _context;

        public UserRepository(EFDataContext context) {
            _context = context;
        }

        public ICollection<User> GetUsers() {
            return _context.Users.Include(x => x.UserRoles).ThenInclude(ur => ur.Role).ToList();
        }

        public ICollection<GetUserDTO> GetUsersDto() {
            var usersWithRoles = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .GroupBy(u => new {
                    u.Id,
                    u.Name,
                    u.Age,
                    u.Email
                })
                .Select(g => new GetUserDTO {
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    Age = g.Key.Age,
                    Email = g.Key.Email,
                    Roles = g.SelectMany(ur => ur.UserRoles.Select(role => role.Role.Type)).ToList()
                })
                .ToList();
            return usersWithRoles;
        }

        public GetUserDTO GetUserDto(int id) {
            return GetUsersDto().Where(u => u.Id == id).FirstOrDefault();
        }

        public User GetUser(int id) {
            return _context.Users.Where(p => p.Id == id).FirstOrDefault();
        }

        public User GetUser(string email) {
            return _context.Users.Where(p => p.Email == email).FirstOrDefault();
        }

        public bool UserExists(int id) {
            return _context.Users.Any(u => u.Id == id);
        }
        
        public bool CreateUser(User user) {
            _context.Add(user);
            return Save();
        }

        public bool UpdateUser(User user) {
            _context.Update(user);
            return Save();
        }

        public bool DeleteUser(User user) {
            _context.Remove(user);
            return Save();
        }

        public bool Save() {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
