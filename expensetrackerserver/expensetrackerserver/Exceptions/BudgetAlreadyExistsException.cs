namespace expensetrackerserver.Exceptions
{
    public class BudgetAlreadyExistsException : Exception
    {
        public BudgetAlreadyExistsException()
            : base("This budget already exists.")
        {

        }
    }
}
