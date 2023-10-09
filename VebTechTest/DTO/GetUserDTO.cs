using VebTechTest.Models;

namespace VebTechTest.DTO {
    public class GetUserDTO {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; }
    }
}
