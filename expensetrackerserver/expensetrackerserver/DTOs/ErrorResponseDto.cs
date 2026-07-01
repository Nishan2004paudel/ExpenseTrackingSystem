namespace expensetrackerserver.DTOs
{
    public class ErrorResponseDto
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Errors { get; set; }
    }
}
