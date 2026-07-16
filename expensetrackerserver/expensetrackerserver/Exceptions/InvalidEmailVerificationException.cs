namespace expensetrackerserver.Exceptions
{
    public class InvalidEmailVerificationException : Exception
    {
        public InvalidEmailVerificationException(string message)
            : base(message)
        {

        }
    }
}
