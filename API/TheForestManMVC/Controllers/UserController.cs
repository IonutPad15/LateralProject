using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Models.Request;
using Models.Response;
using System.Net;
using TheForestManMVC.Models;


namespace TheForestManMVC.Controllers
{
    public class UserController : Controller
    {
       
        string url = "http://localhost:5083/api";
        public async Task<IActionResult> Index()
        {
            List<UserInfo> users = await GetAllUsers();
            return View(users);
        }
        
        public async Task<List<UserInfo>> GetAllUsers()
        {
            List<UserInfo> userInfos = new List<UserInfo>();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync($"{url}/user");
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserInfo>>();
                    foreach (var user in users)
                    {
                        Console.WriteLine(user.ToString());
                    }
                    return users.ToList();
                }
            }
            return userInfos;
        }
        //[HttpPost]
        //public ActionResult Create(string userName, string email, string password, string repeatPassword)
        //{
        //    if(!password.Equals(repeatPassword)) return View("../Home/Index");
        //    UserModel userModel = new UserModel()
        //    {
        //        UserName = userName,
        //        Email = email,
        //        Password = password
        //    };
        //    {
        //        Console.WriteLine("\n\n\n\n");
        //        Console.WriteLine(userModel.UserName + userModel.Email + userModel.Password);
        //        Console.WriteLine("\n\n\n\n");
        //    }
        //    return View(userModel);
        //}
        //[HttpPost]
        //public ActionResult Create(string UserName, string Email, string Password, string RepeatPassword)
        //{

        //    UserModel userModel = new UserModel()
        //    {
        //        UserName = UserName,
        //        Email = Email,
        //        Password = Password
        //    };
        //    {
        //        Console.WriteLine("\n\n\n\n");
        //        Console.WriteLine(userModel.UserName + userModel.Email + userModel.Password);
        //        Console.WriteLine("\n\n\n\n");
        //    }
        //    return View(userModel);
        //}
        [HttpPost]
        public async Task<ActionResult> Create(RegisterModel registerModel)
        {

            UserModel userModel = new UserModel()
            {
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                Password = registerModel.Password
            };
            {
                Console.WriteLine("\n\n\n\n");
                Console.WriteLine(userModel.UserName + userModel.Email + userModel.Password);
                Console.WriteLine("\n\n\n\n");
            }
            using (HttpClient client = new HttpClient())
            {
                UserModel credentials = new UserModel();
                var respuesta = await client.GetAsync($"{url}/user/registercode?username={userModel.UserName}&&email={userModel.Email}");
                if (respuesta.StatusCode == HttpStatusCode.InternalServerError)
                {
                    respuesta.EnsureSuccessStatusCode();
                }
                return View(respuesta);
            }
            return View(userModel);
        }

        public async Task<IActionResult> CreateUser()
        {
           
            return View();
            
        }
    }
}
