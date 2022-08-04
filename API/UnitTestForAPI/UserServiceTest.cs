

using API.Utils;
using Microsoft.EntityFrameworkCore;

namespace UnitTestForAPI
{
    [TestClass]
    public class UserServiceTest
    {
        private readonly string url = "https://localhost:7083/api";
        private readonly SiteDbContext _context;
        DbContextOptions<SiteDbContext> options = new DbContextOptionsBuilder<SiteDbContext>()
          .UseInMemoryDatabase(databaseName: "SiteDataBase")
          .Options;
        HashHelper hashHelper = new HashHelper();
        UserService userService = new UserService();
        
        public TestContext TestContext { get; set; }

        public UserServiceTest()
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
        public async void TestCreateUserSucces()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async void TestCreateUserError()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async void TestGetUsers()
        {
            Assert.Inconclusive();
        }
    }
}