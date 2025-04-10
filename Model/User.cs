using ASP_NET.Enum;
using System.ComponentModel.DataAnnotations;

namespace ASP_NET.Model
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; } // ENUM

    }
}
