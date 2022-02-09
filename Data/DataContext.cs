using if3250_2022_01_buletin_backend.Models;

namespace if3250_2022_01_buletin_backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; } = null!;
    }

}