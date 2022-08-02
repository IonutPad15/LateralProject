using API.Models;
using API.Services;

namespace API.Data.IRepository
{
    public interface ICommentService
    {
        Task<Comment?> GetCommentById(SiteDbContext _context, Guid id);
        Task<DbCodes.Codes> CreateComment(SiteDbContext _context, Comment comment);
        Task<DbCodes.Codes> UpdateComment(SiteDbContext _context, Comment comment);
        
    }
}
