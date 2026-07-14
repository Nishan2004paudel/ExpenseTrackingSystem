namespace expensetrackerserver.Exceptions
{
    public class InvalidRefreshTokenException : Exception
    {
        public InvalidRefreshTokenException(string message)
            : base(message)
        {

        }
    }
}
