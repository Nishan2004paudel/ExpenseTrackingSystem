namespace expensetrackerserver.Exceptions
{
    public class ExpenseAccessDeniedException : Exception
    {
        public ExpenseAccessDeniedException(string message)
            : base(message)
        {

        }
    }
}
