namespace VebTechTest.Models {
    public class Role {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
