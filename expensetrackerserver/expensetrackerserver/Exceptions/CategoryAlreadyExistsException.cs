namespace expensetrackerserver.Exceptions
{
    public class CategoryAlreadyExistsException: Exception
    {
        public CategoryAlreadyExistsException()
            :base("This category already exists.")
        {
            
        }
    }
}
