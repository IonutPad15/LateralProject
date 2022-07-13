using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Data
{
    //TODO: improper name, doesn't describe the context
    public class SiteDbContext: DbContext
    {
        public SiteDbContext(DbContextOptions<SiteDbContext> options)
            : base(options)
        {
        }

        //TODO: confusing comment

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(entity => {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.UserName).IsUnique();
            });
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
