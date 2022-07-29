using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Models.Response;

namespace API.Services
{
    public class UserService
    {
        public static async Task<List<UserInfo>> GetUsers(SiteDbContext _context)
        {
            
            var userinfo = await _context.Users.Where(u=> u.IsDeleted == false)
                .Select(u => new UserInfo()
            {
                UserName = u.UserName,
                Email = u.Email,
                Id = u.Id
            }).ToListAsync();
            return userinfo;
        }
        public static async Task<User?> GetUserById(SiteDbContext _context, Guid? id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.IsDeleted == false && u.Id == id);
        }
        public static async Task<User?> GetPostsAndCommentsByUserId(SiteDbContext _context, Guid id)
        {
            var usertester = await _context.Users.Include(x => x.Posts)
                                           .ThenInclude(x => x.Comments)
                                           .SingleAsync(x => x.Id == id && x.IsDeleted == false);
            if (usertester == null) return null;
            var usercomments = _context.Users.Include(x => x.Comments)
                                                .Single(x => x.Id == id && x.IsDeleted == false);
            return usertester;
        }
        public static async Task<User?> GetUserByUsername(SiteDbContext _context, string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username && u.IsDeleted == false);
        }
        public static async Task<User?> GetUserByUsernameAndEmail(SiteDbContext _context, string username, string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username && u.IsDeleted == false && u.Email == email);
        }
        public static async Task<DbCodes.Codes> CreateUser(SiteDbContext _context, User user)
        {
            try
            {
                await _context.Users.AddAsync(user);

                await _context.SaveChangesAsync();
                return DbCodes.Codes.Succes;
            }
            catch
            {
                return DbCodes.Codes.Error;
            }
        }
        public static async Task<DbCodes.Codes> UpdateUser(SiteDbContext _context, User user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
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
