using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Models.Response;
using AutoMapper;
using Models.Request;

namespace API.Services
{
    public class UserService
    {
        private static readonly MapperConfiguration? config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<User, UserInfo>()
                );
        public UserService()
        {
            
        }
        public static async Task<List<UserInfo>> GetUsers(SiteDbContext _context)
        {

            //var userinfo = await _context.Users.Where(u=> u.IsDeleted == false)
            //    .Select(u => new UserInfo()
            //{
            //    UserName = u.UserName,
            //    Email = u.Email,
            //    Id = u.Id
            //}).ToListAsync();
            //return userinfo;
            var users = await _context.Users.Where(u => u.IsDeleted == false).ToListAsync();
            var mapper = new Mapper(config);
            return mapper.Map<List<UserInfo>>(users);
        }
        public static async Task<User?> GetUserById(SiteDbContext _context, Guid? id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.IsDeleted == false && u.Id == id);
        }
        public static async Task<User?> GetUserWithPostsAndCommentsByUserId(SiteDbContext _context, Guid id)
        {
            var usertester = await _context.Users.Include(x => x.Posts.Where(p => p.IsDeleted == false).OrderByDescending(p => p.Updated))
                                           .ThenInclude(x => x.Comments.Where(c => c.IsDeleted == false).OrderByDescending(c => c.Updated))
                                           .Include(x => x.Comments.Where(c => c.IsDeleted == false).OrderByDescending(c => c.Updated))
                                           .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
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
        public static async Task<User?> GetUserByCredentials(SiteDbContext _context, Credentials credentials)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.IsDeleted == false
            && x.Password == credentials.Password
            && (x.Email == credentials.NameEmail || x.UserName == credentials.NameEmail));
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
