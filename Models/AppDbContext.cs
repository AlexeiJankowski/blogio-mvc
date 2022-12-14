using Microsoft.EntityFrameworkCore;

namespace Blogio.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Comment> Comments => Set<Comment>();
    }
}