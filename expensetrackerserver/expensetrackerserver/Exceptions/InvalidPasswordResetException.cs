namespace expensetrackerserver.Exceptions
{
    public class InvalidPasswordResetException:Exception
    {
        public InvalidPasswordResetException(string message)
            :base(message)
        {
            
        }
    }
}
