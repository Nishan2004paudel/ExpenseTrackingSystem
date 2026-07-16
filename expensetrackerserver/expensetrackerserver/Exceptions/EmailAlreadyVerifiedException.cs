namespace expensetrackerserver.Exceptions
{
    public class EmailAlreadyVerifiedException : Exception
    {
        public EmailAlreadyVerifiedException(string message)
            : base(message)
        {

        }
    }
}
