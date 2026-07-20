namespace expensetrackerserver.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? Profession { get; set; }

        public string PreferredCalendar { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string Role { get; set; } = "User";
        public int TokenVersion { get; set; }
        public bool IsEmailVerified { get; set; }
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationExpiresAt { get; set; }
        public string AuthProvider { get; set; } = "Local";
    }
}
