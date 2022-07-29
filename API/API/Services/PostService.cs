using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Models.Response;
namespace API.Services
{
    public class PostService
    {
        public static async Task<List<Post>> GetPosts(SiteDbContext _context)
        {
            return await _context.Posts.Include(x => x.Comments)
                .Where(x => x.IsDeleted == false).OrderByDescending(p => p.Updated).ToListAsync();
        }
        public static async Task<Post> GetPostById(SiteDbContext _context, Guid id)
        {
            return await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
        }
        public static async Task<Post?> GetCommentsByPostId(SiteDbContext _context, Guid id)
        {
            return await _context.Posts.Include(x => x.Comments).FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
        }
        public static async Task<DbCodes.Codes> CreatePost(SiteDbContext _context, Post post)
        {
            try
            {
                await _context.Posts.AddAsync(post);
                await _context.SaveChangesAsync();
                return DbCodes.Codes.Succes;
            }
            catch
            {
                return DbCodes.Codes.Error;
            }
        }
        public static async Task<DbCodes.Codes> UpdatePost(SiteDbContext _context, Post post)
        {
            try
            {
                _context.Entry(post).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return DbCodes.Codes.Succes;
            }
            catch
            {
                return DbCodes.Codes.Error;
            }
        }
       
    }
}
