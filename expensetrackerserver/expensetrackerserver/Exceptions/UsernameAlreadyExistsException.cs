namespace expensetrackerserver.Exceptions
{
    public class UsernameAlreadyExistsException: Exception
    {
        public UsernameAlreadyExistsException()
            :base("Username already exists.")
        {
            
        }
    }
}
