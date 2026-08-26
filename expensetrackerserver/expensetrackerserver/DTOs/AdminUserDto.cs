namespace expensetrackerserver.DTOs
{
    public class AdminUserDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Profession { get; set; }
        public string PreferredCalendar { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string AuthProvider { get; set; } = string.Empty;
        public bool IsEmailVerified { get; set; }
        public bool IsActive { get; set; }
        public int? DeactivatedBy { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        public string? DeactivationReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
