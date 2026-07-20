namespace expensetrackerserver.Exceptions
{
    public class PasswordAlreadySetException : Exception
    {
        public PasswordAlreadySetException()
            : base("Password has already been set.")
        {

        }
    }
}
