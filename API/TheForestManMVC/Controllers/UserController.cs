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
        private static UserModel Credentials = new UserModel();

        
        public async Task<ActionResult> Index()
        {
           
            List<UserInfo> users = await GetAllUsers();
            return View(users);
            Credentials = new UserModel();
        }
        
        private async Task<List<UserInfo>> GetAllUsers()
        {
            List<UserInfo> userInfos = new List<UserInfo>();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync($"{HomeController.url}/user");
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
        [HttpPost]
        public async Task<ActionResult> Register(string code)
        {
            ValidationCode coderead = new ValidationCode()
            {
                Code = code,
                Created = DateTime.Now
            };
            UserCode userCode = new UserCode()
            {
                Email = Credentials.Email,
                Password = Credentials.Password,
                UserName = Credentials.UserName,
                Code = coderead
            };
            using (HttpClient client = new HttpClient())
            {
                var respueste = await client.PostAsJsonAsync($"{HomeController.url}/user/create", userCode);
                if (respueste.IsSuccessStatusCode)
                    return RedirectToAction("Index", "User");
                else
                {
                    ViewBag.ErrorUser = "A aparut o eroare";
                    return View("GetRegisterCode");
                }
                
                
            }
            
        }
        private async Task<bool> GetCode()
        {
            using (HttpClient client = new HttpClient())
            {
                var respuesta = await client.GetAsync($"{HomeController.url}/user/registercode?username={Credentials.UserName}&&email={Credentials.Email}");

                if (respuesta.IsSuccessStatusCode)
                    return true;
               return false;

            }
        }
        public async Task<ActionResult> ResendCode()
        {
            var code = await GetCode();
            if(!code)
            {
                ViewBag.ErrorUser = "A aparut o eroare";
            }
            return View("GetRegisterCode");
            
        }
        [HttpPost]
        public async Task<ActionResult> GetRegisterCode(RegisterModel registerModel)
        {
            Credentials.UserName = registerModel.UserName;
            Credentials.Email = registerModel.Email;
            Credentials.Password = registerModel.Password;

            var code = await GetCode();
            if(code == false)
            {
                ViewBag.ErrorCreateUser = "There is an account with this username/email or the password is too short";
                return View("CreateUser");
            }
            return View();
        }

        public async Task<IActionResult> CreateUser()
        {
           
            return View();
            
        }
    }
}
