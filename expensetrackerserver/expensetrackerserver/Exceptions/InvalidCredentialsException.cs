namespace expensetrackerserver.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException()
            : base("Invalid username/email or password.")
        {

        }
    }
}
