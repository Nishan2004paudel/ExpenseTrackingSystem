namespace expensetrackerserver.Exceptions
{
    public class BudgetNotFoundException : Exception
    {
        public BudgetNotFoundException()
            : base("The required budget limit didn't found.")
        {

        }
    }
}
