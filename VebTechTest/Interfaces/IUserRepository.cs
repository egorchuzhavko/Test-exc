using VebTechTest.DTO;
using VebTechTest.Models;

namespace VebTechTest.Interfaces {
    public interface IUserRepository {
        ICollection<User> GetUsers();
        ICollection<GetUserDTO> GetUsersDto();
        GetUserDTO GetUserDto(int id);
        User GetUser(int id);
        User GetUser(string email);
        bool UserExists(int id);
        bool CreateUser(User user);
        bool UpdateUser(User user);
        bool DeleteUser(User user);
        bool Save();
    }
}
