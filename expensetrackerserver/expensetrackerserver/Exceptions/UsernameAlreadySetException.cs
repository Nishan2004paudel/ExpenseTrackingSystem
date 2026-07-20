namespace expensetrackerserver.Exceptions
{
    public class UsernameAlreadySetException : Exception
    {
        public UsernameAlreadySetException()
            :base("Username has already been set.")
        {

        }
    }
}
