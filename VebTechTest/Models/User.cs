using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VebTechTest.Models {
    [Table("user")]
    public class User {
        [Key,Required]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public List<Role> Roles { get; set; } = new List<Role>();
    }
}
