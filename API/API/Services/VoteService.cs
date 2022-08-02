using API.Data;
using API.Data.IRepository;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class VoteService:IVoteService
    {

        public async Task<Vote?> GetVoteByUserAndCommentId(SiteDbContext _context, Guid? userId, Guid? commentId)
        {
            var vote = await _context.Votes.FirstOrDefaultAsync(v => v.UserId == userId && v.CommentId == commentId);
            return vote;
        }
        public async Task<Vote?> GetVoteByUserAndPostId(SiteDbContext _context, Guid? userId, Guid? postId)
        {
            var vote = await _context.Votes.FirstOrDefaultAsync(v => v.UserId == userId && v.PostId == postId);
            return vote;
        }
        public async Task<DbCodes.Codes> CreateVote(SiteDbContext _context, Vote vote)
        {
            try
            {
                await _context.Votes.AddAsync(vote);

                await _context.SaveChangesAsync();
                return DbCodes.Codes.Succes;
            }
            catch
            {
                return DbCodes.Codes.Error;
            }
        }
        public async Task<DbCodes.Codes> UpdateVote(SiteDbContext _context, Vote vote)
        {
            try
            {
                _context.Entry(vote).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return DbCodes.Codes.Succes;
            }
            catch
            {
                return DbCodes.Codes.Error;
            }
        }
        public async Task<DbCodes.Codes> DeleteVote(SiteDbContext _context, Vote vote)
        {
            try
            {
                _context.Entry(vote).State = EntityState.Deleted;
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
