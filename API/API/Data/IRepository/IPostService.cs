using API.Models;
using API.Services;

namespace API.Data.IRepository
{
    public interface IPostService
    {
        Task<List<Post>> GetPosts(SiteDbContext _context);

        Task<Post?> GetPostById(SiteDbContext _context, Guid id);

        Task<Post?> GetPostWithCommentsByPostId(SiteDbContext _context, Guid id);
        Task<DbCodes.Codes> CreatePost(SiteDbContext _context, Post post);
        Task<DbCodes.Codes> UpdatePost(SiteDbContext _context, Post post);
       
    }
}
