using Microsoft.EntityFrameworkCore;

namespace EfCoreBenchmarksApp
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }

        public async Task SeedData(int userCount = 100, int ordersPerUser = 1000)
        {
            var random = new Random();

            var users = Enumerable.Range(0, userCount).Select(
                userIndex => new User
                {
                    UserId = $"user-{userIndex}",
                    Orders = (ICollection<Order>)Enumerable.Range(0, ordersPerUser).Select(orderIndex =>
                        new Order
                        {
                            OrderId = $"order-{userIndex}-{orderIndex}",
                            TotalAmount = random.Next(10, 100),
                            OrderDate = DateTime.UtcNow.AddDays(random.Next(1, 7) * -1)
                        })
                });

            Users.AddRange(users);
            await SaveChangesAsync();
        }
    }
}
