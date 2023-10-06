using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VebTechTest.Models {
    [Table("role")]
    public class Role {
        [Key,Required]
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public List<User> Users { get; set; } = new List<User>();
    }
}
