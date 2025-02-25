namespace EfCoreBenchmarksApp
{
    public class Order
    {
        public DateTime OrderDate { get; set; }
        public string OrderId { get; set; } = default!;
        public decimal TotalAmount { get; set; }
    }

    public class User
    {
        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
        public string UserId { get; set; } = default!;
    }
}