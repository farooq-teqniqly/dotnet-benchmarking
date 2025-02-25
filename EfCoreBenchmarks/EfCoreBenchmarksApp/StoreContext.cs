using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EfCoreBenchmarksApp
{
    public class StoreContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
