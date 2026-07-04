namespace expensetrackerserver.Exceptions
{
    public class ExpenseNotFoundException: Exception
    {
        public ExpenseNotFoundException()
            :base("The requested expense is not being found.")
        {
            
        }
    }
}
