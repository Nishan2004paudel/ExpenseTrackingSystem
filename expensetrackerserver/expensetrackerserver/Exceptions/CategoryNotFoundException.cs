namespace expensetrackerserver.Exceptions
{
    public class CategoryNotFoundException : Exception
    {
        public CategoryNotFoundException()
            : base("The requested category is not being found.")
        {

        }
    }
}
