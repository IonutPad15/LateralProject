using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Models.Response;
using AutoMapper;
using Models.Request;
using API.Data.IRepository;

namespace API.Services
{
    public class UserService:IUserService
    {
        private static readonly MapperConfiguration? config = new MapperConfiguration(cfg =>
                    cfg.CreateMap<User, UserInfo>()
                );
        private readonly SiteDbContext context = new SiteDbContext();
        public UserService()
        {
            
        }
        public async Task<List<UserInfo>> GetUsers(SiteDbContext _context)
        {
            var users = await _context.Users.Where(u => u.IsDeleted == false).ToListAsync();
            var mapper = new Mapper(config);
            return mapper.Map<List<UserInfo>>(users);
        }
        public async Task<List<UserInfo>> GetUsers()
        {
            var users = await context.Users.Where(u => u.IsDeleted == false).ToListAsync();
            var mapper = new Mapper(config);
            return mapper.Map<List<UserInfo>>(users);
        }
        public async Task<User?> GetUserById(SiteDbContext _context, Guid? id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.IsDeleted == false && u.Id == id);
        }
        public async Task<User?> GetUserWithPostsAndCommentsByUserId(SiteDbContext _context, Guid id)
        {
            var usertester = await _context.Users.Include(x => x.Posts.Where(p => p.IsDeleted == false).OrderByDescending(p => p.Updated))
                                           .ThenInclude(x => x.Comments.Where(c => c.IsDeleted == false).OrderByDescending(c => c.Updated))
                                           .Include(x => x.Comments.Where(c => c.IsDeleted == false).OrderByDescending(c => c.Updated))
                                           .FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false);
            return usertester;
        }
        public async Task<User?> GetUserByUsername(SiteDbContext _context, string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username && u.IsDeleted == false);
        }
        public async Task<User?> GetUserByUsernameAndEmail(SiteDbContext _context, string username, string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username && u.IsDeleted == false && u.Email == email);
        }
        public async Task<User?> GetUserByCredentials(SiteDbContext _context, Credentials credentials)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.IsDeleted == false
            && x.Password == credentials.Password
            && (x.Email == credentials.NameEmail || x.UserName == credentials.NameEmail));
        }
        public async Task<DbCodes.Codes> CreateUser(SiteDbContext _context, User user)
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
        public async Task<DbCodes.Codes> UpdateUser(SiteDbContext _context, User user)
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
