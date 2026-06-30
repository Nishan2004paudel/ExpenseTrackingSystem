namespace expensetrackerserver.DTOs
{
    public class LoginResponseDto
    {
        public UserDetailDto User { get; set; } = new UserDetailDto();

        public string Message { get; set; } = string.Empty;

        public string? Token { get; set; }
    }
}
