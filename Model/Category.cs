namespace ASP_NET.Model
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Các trường user tracking
        public Guid UserCreate { get; set; }
        public Guid? UserUpdate { get; set; }
        public Guid? UserDelete { get; set; }

        // Quan hệ ngược với Product
        public ICollection<Product> Products { get; set; }
    }
}
