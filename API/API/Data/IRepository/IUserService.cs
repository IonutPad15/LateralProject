using API.Models;
using API.Services;
using Models.Request;
using Models.Response;

namespace API.Data.IRepository
{
    public interface IUserService
    {
        Task<List<UserInfo>> GetUsers(SiteDbContext _context);

        Task<User?> GetUserById(SiteDbContext _context, Guid? id);

        Task<User?> GetUserWithPostsAndCommentsByUserId(SiteDbContext _context, Guid id);

        Task<User?> GetUserByUsername(SiteDbContext _context, string username);

        Task<User?> GetUserByUsernameAndEmail(SiteDbContext _context, string username, string email);

        Task<User?> GetUserByCredentials(SiteDbContext _context, Credentials credentials);

        Task<DbCodes.Codes> CreateUser(SiteDbContext _context, User user);

        Task<DbCodes.Codes> UpdateUser(SiteDbContext _context, User user);
        
    }
}
