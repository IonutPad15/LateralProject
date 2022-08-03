

using API.Utils;
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
        DbContextOptions<SiteDbContext> options = new DbContextOptionsBuilder<SiteDbContext>()
          .UseInMemoryDatabase(databaseName: "SiteDataBase")
          .Options;
        HashHelper hashHelper = new HashHelper();
        UserService userService = new UserService();
        
        public UnitTest1()
        {
            _context = new SiteDbContext(options);
            string hashedpass = hashHelper.GetHash("12345678");

            User user = new User()
            {
                UserName = "Mihai",
                Email = "mihai@yahoo.com",
                Password = hashedpass
            };
            _context.Users.Add(user);
            _context.SaveChanges();


        }
        [TestMethod]
        public async Task TestCreateUser()
        {
            string hashedpass = hashHelper.GetHash("12345678");
            User user = new User()
            {
                UserName = "Ionut",
                Email = "ionut@gmail.com",
                Password = hashedpass
            };
            var codeResult = await userService.CreateUser(_context, user);
        }
        [TestMethod]
        public async Task TestGetUsers()
        {
            
            

            List<UserInfo> userInfos = await userService.GetUsers(_context);
        }
    }
}