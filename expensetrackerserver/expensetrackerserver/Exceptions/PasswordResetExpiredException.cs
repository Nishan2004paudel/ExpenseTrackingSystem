namespace expensetrackerserver.Exceptions
{
    public class PasswordResetExpiredException : Exception
    {
        public PasswordResetExpiredException(string message)
            : base(message)
        {

        }
    }
}
