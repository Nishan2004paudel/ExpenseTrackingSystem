namespace expensetrackerserver.Exceptions
{
    public class InvalidPreferredCalendarException : Exception
    {
        public InvalidPreferredCalendarException()
            :base("Preferred calendar must be either English or Nepali")
        {
            
        }
    }
}
