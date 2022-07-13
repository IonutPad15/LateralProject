using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Data
{
    //TODO: improper name, doesn't describe the context
    public class TFMDbContext: DbContext
    {
        public TFMDbContext(DbContextOptions<TFMDbContext> options)
            : base(options)
        {
        }

        //TODO: confusing comment
        //3 tabele in TFMDbContext. TFM vine de la the forest man
        public DbSet<UserModel> Users { get; set; }
        public DbSet<PostModel> Posts { get; set; }
        public DbSet<CommentModel> Comments { get; set; }
    }
}
