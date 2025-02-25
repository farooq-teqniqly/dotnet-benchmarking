using Microsoft.EntityFrameworkCore;

namespace EfCoreBenchmarksApp
{
    public class StoreContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }

        public async Task SeedData(int userCount = 100, int ordersPerUser = 1000)
        {
            var random = new Random();

            var users = Enumerable.Range(0, userCount).Select(
                userIndex => new User
                {
                    UserId = $"user-{userIndex + 1}",
                    IsActive = random.Next(2) == 0,
                    Orders = Enumerable.Range(0, ordersPerUser).Select(orderIndex =>
                        new Order
                        {
                            OrderId = $"order-{userIndex + 1}-{orderIndex + 1}",
                            TotalAmount = random.Next(10, 100),
                            OrderDate = DateTime.UtcNow.AddDays(random.Next(1, 8) * -1)
                        }).ToList()
                });

            Users.AddRange(users);
            await SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                $"Server=localhost,{ConfigurationService.GetDbPort()};Database=benchmark-efcore;User Id=sa;Password={ConfigurationService.GetDbPassword()};TrustServerCertificate=true");
        }
    }
}
