using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EfCoreBenchmarksApp
{
    public class StoreContext : DbContext
    {
        private readonly string _database;
        private readonly string _password;
        private readonly string _userName;

        public StoreContext(string userName, string password, string database)
        {
            _userName = userName;
            _password = password;
            _database = database;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var sqlConnectionStringBuilder = new SqlConnectionStringBuilder { UserID = _userName, Password = _password, InitialCatalog = _database };
            optionsBuilder.UseSqlServer(sqlConnectionStringBuilder.ToString());
            base.OnConfiguring(optionsBuilder);
        }
    }
}
