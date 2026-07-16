namespace expensetrackerserver.Exceptions
{
    public class EmailNotVerifiedException : Exception
    {
        public EmailNotVerifiedException(string message)
            : base(message)
        {

        }
    }
}
