namespace expensetrackerserver.Exceptions
{
    public class PastBudgetMonthNotAllowedException:Exception
    {
        public PastBudgetMonthNotAllowedException(string message)
            :base(message)
        {
            
        }
    }
}
