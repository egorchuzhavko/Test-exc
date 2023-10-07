using VebTechTest.EFCore;
using VebTechTest.Models;
using VebTechTest.Interfaces;

namespace VebTechTest.Repository {
    public class UserRepository : IUserRepository {
        private readonly EFDataContext _context;

        public UserRepository(EFDataContext context) {
            _context = context;
        }

        public ICollection<User> GetUsers() {
            return _context.Users.OrderBy(x => x.Id).ToList();
        }

        public User GetUser(int id) {
            return _context.Users.Where(p => p.Id == id).FirstOrDefault();
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
