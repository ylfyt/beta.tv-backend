using if3250_2022_01_buletin_backend.src.Models;

namespace if3250_2022_01_buletin_backend.src.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<User> User { get; set; } = null!;
    }

}