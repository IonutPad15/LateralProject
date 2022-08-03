using API.Data;
using API.Data.IRepository;
using API.Models;
using Microsoft.EntityFrameworkCore;
namespace API.Services
{
    public class PostService: IPostService
    {
        public async Task<List<Post>> GetPosts(SiteDbContext _context)
        {
            return await 
            _context.Posts.Include(p=>p.Votes).
            Include(x => x.Comments.Where(c =>  c.IsDeleted == false).OrderByDescending(c=> c.Updated))
            .ThenInclude(c => c.Votes)
            .Where(p => p.IsDeleted == false).OrderByDescending(p => p.Updated).ToListAsync();
        }
        public async Task<Post?> GetPostById(SiteDbContext _context, Guid id)
        {
            return await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
        }
        public async Task<Post?> GetPostWithCommentsByPostId(SiteDbContext _context, Guid id)
        {
            return await _context.Posts.Include(p => p.Votes).Include(x => x.Comments.Where(c => c.IsDeleted == false).OrderByDescending(c=>c.Updated))
                .ThenInclude(c=>c.Votes)
            .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
        }
        public async Task<DbCodes.Codes> CreatePost(SiteDbContext _context, Post post)
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
        public async Task<DbCodes.Codes> UpdatePost(SiteDbContext _context, Post post)
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
