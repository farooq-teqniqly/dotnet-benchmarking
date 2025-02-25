using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CA1822

namespace EfCoreBenchmarksApp
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        [Params(1000)]
        public int NumUsers;

        [Params(100)]
        public int OrdersPerUser;

        [Benchmark]
        public async Task<double> LoadEntities()
        {
            var totalAmount = 0m;
            var numOrders = 0;

            await using (var context = new StoreContext())
            {
                await foreach (var order in context.Orders.AsAsyncEnumerable())
                {
                    totalAmount += order.TotalAmount;
                    numOrders++;
                }
            }

            return (double)totalAmount / numOrders;
        }

        [Benchmark]
        public async Task<double> LoadEntitiesNoTracking()
        {
            var totalAmount = 0m;
            var numOrders = 0;

            await using (var context = new StoreContext())
            {
                await foreach (var order in context.Orders.AsNoTracking().AsAsyncEnumerable())
                {
                    totalAmount += order.TotalAmount;
                    numOrders++;
                }
            }

            return (double)totalAmount / numOrders;
        }

        [Benchmark]
        public async Task<decimal> LoadRelatedEntitiesEfficiently()
        {
            var totalAmount = 0m;

            await using (var context = new StoreContext())
            {
                var usersWithOrders = context.Users.Include(u => u.Orders).ToList();

                foreach (var user in usersWithOrders)
                {
                    totalAmount += user.Orders.Sum(o => o.TotalAmount);
                }
            }

            return totalAmount;
        }

        [Benchmark]
        public async Task<decimal> LoadRelatedEntitiesInefficiently()
        {
            var totalAmount = 0m;

            await using (var context = new StoreContext())
            {
                var users = context.Users.ToList();

                foreach (var user in users)
                {
                    totalAmount += context.Orders.Where(o => o.UserId == user.UserId).Sum(o => o.TotalAmount);
                }
            }

            return totalAmount;
        }

        [GlobalSetup]
        public async Task Setup()
        {
            await using (var context = new StoreContext())
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
                await context.SeedData(NumUsers, OrdersPerUser);
            }
        }
    }
}
