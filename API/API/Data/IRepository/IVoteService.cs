using API.Models;
using API.Services;

namespace API.Data.IRepository
{
    public interface IVoteService
    {
        Task<Vote?> GetVoteByUserAndCommentId(SiteDbContext _context, Guid? userId, Guid? commentId);
        Task<Vote?> GetVoteByUserAndPostId(SiteDbContext _context, Guid? userId, Guid? postId);
        Task<DbCodes.Codes> CreateVote(SiteDbContext _context, Vote vote);
        Task<DbCodes.Codes> UpdateVote(SiteDbContext _context, Vote vote);
        Task<DbCodes.Codes> DeleteVote(SiteDbContext _context, Vote vote);
    }
}
