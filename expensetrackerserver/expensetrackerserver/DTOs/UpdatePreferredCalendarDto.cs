using System.ComponentModel.DataAnnotations;

namespace expensetrackerserver.DTOs
{
    public class UpdatePreferredCalendarDto
    {
        [Required]
        public string PreferredCalendar { get; set; } = string.Empty;
    }
}
