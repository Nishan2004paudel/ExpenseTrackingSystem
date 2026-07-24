namespace expensetrackerserver.DTOs
{
    public class ChangeUsernameDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewUsername { get; set; } = string.Empty;
    }
}
