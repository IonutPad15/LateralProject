using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Data
{

    public class SiteDbContext: DbContext
    {
        public SiteDbContext(DbContextOptions<SiteDbContext> options)
            : base(options)
        {
        }
        public SiteDbContext()
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(entity => {
                
                entity.HasIndex(e => new { e.Email, e.IsDeleted }).IsUnique();
                entity.HasIndex(e => new { e.UserName, e.IsDeleted }).IsUnique();
            });
            //builder.Entity<Post>(entity =>
            //{
            //    entity.HasOne(u => u.User)
            //    .WithMany(p => p.Posts)
            //    .HasForeignKey(ui => ui.UserId)
            //    .OnDelete(DeleteBehavior.ClientSetNull);
            //});
            //)
            //    .HasOne(u => u.User)
            //    .WithMany(p => p.Posts)
            //    .HasForeignKey(ui => ui.UserId);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
