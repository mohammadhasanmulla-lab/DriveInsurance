using System.ComponentModel.DataAnnotations;

namespace MyProject.Models
{
    /// <summary>
    /// Persisted user entity — stored in the Users table.
    /// Passwords are hashed with BCrypt before storage.
    /// </summary>
    public class AppUser
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(60)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string Role { get; set; } = "Customer"; // Customer | ClaimsOfficer | Surveyor | ComplianceOfficer | Admin

        [MaxLength(80)]
        public string Department { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Status { get; set; } = "Active"; // Active | Inactive

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    // ── View models used by auth forms ────────────────────────────────────────

    public class RegisterViewModel
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(60)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6), MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required, Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Customer";
    }
}
