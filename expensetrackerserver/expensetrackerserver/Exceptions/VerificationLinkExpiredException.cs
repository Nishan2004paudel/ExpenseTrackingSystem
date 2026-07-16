namespace expensetrackerserver.Exceptions
{
    public class VerificationLinkExpiredException : Exception
    {
        public VerificationLinkExpiredException(string message)
            : base(message)
        {

        }
    }
}
