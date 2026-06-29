

namespace expensetrackerserver.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        public int UserId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
