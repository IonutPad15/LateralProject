using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Models.Request;
using Models.Response;
using System.Net;
using TheForestManMVC.Models;
using System.Text.Json;

namespace TheForestManMVC.Controllers
{
    public class UserController : Controller
    {
        private static UserModel registerCredentials = new UserModel();
        public static Credentials loginCredentials = new Credentials();
        public static UserToken token;
        public async Task<ActionResult> Index()
        {
            List<UserInfo> users = await GetAllUsers();
            return View(users);
            
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
                Email = registerCredentials.Email,
                Password = registerCredentials.Password,
                UserName = registerCredentials.UserName,
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
                var respuesta = await client.GetAsync($"{HomeController.url}/user/registercode?username={registerCredentials.UserName}&&email={registerCredentials.Email}");

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
            if(!registerModel.Password.Equals(registerModel.RepeatPassword))
            {
                ViewBag.ErrorCreateUser = "The passwords are not the same";
                return View("CreateUser");
            }
            registerCredentials.UserName = registerModel.UserName;
            registerCredentials.Email = registerModel.Email;
            registerCredentials.Password = registerModel.Password;

            var code = await GetCode();
            if(code == false)
            {
                ViewBag.ErrorCreateUser = "There is an account with this username/email or the password is too short";
                return View("CreateUser");
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> LogInAction(Credentials credentials)
        {
            using (HttpClient client = new HttpClient())
            {
                var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                HttpResponseMessage httpResponseToken = await client.PostAsJsonAsync($"{HomeController.url}/user/login", credentials);
                if(httpResponseToken.StatusCode == HttpStatusCode.BadRequest)
                {
                    ViewBag.ErrorUser = "Invalid log in attempt";
                    return View("LogIn");
                }
                var responseToken = JsonSerializer.Deserialize<UserToken>(await
                    httpResponseToken.Content.ReadAsStringAsync(), jsonSerializerOptions);
                token = responseToken;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                    responseToken.Token);

                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<ActionResult> AboutUser()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"{HomeController.url}/user/{token.UserId}/postscomments");
                if (response.IsSuccessStatusCode)
                {
                    var user3 = await response.Content.ReadFromJsonAsync<UserPostsCommentsInfo>();
                    return View(user3);
                }
                return Content("nu merge...");
            }
        }
        public async Task<ActionResult> LogIn()
        {
            
            return View();
        }

        public async Task<IActionResult> CreateUser()
        {
           
            return View();
            
        }
    }
}
