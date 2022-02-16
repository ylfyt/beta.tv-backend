using src.Models;

namespace src.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<User> User { get; set; } = null!;
        public DbSet<TokenLog> TokenLogs { get; set; } = null!;
    }

}