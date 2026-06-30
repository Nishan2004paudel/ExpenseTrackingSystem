using System.ComponentModel.DataAnnotations;

namespace expensetrackerserver.DTOs
{
    public class UserDetailDto
    {

        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;


        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string? Profession { get; set; }

        public string PreferredCalendar { get; set; } = string.Empty;

    }
}
