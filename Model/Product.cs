namespace ASP_NET.Model
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Các trường user tracking
        public Guid UserCreate { get; set; }
        public Guid? UserUpdate { get; set; }
        public Guid? UserDelete { get; set; }

        // Quan hệ với Category
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
