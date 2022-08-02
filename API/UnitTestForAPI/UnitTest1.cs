

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

namespace UnitTestForAPI
{
    [TestClass]
    public class UnitTest1
    {
        private readonly string url = "https://localhost:7083/api";
        private readonly SiteDbContext _context;
        private SqliteConnection _connection;
        private DbContextOptionsBuilder _contextOptions;
  
        
        [TestMethod]
        public async Task TestGetUsers()
        {
            UserService userService = new UserService();

            List<UserInfo> userInfos = await userService.GetUsers(_context);
        }
    }
}