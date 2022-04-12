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
        public DbSet<Video> Videos { get; set; } = null!;
        public DbSet<Channel> Channels { get; set; } = null!;
        public DbSet<History> History { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<CommentLike> CommentLikes { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
    }

}
