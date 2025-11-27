using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public enum Role
    {
        User,
        Admin
    }

    public class User
    {
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; } = null!;
        [Required, MaxLength(200)]
        public string Email { get; set; } = null!;
        [Required]
        public string PasswordHash { get; set; } = null!;
        public Role Role { get; set; } = Role.User;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}