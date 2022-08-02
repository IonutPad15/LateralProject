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
                    if (users != null)
                    {
                        
                        return users.ToList();
                    }
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
                {
                    registerCredentials.Password = null;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorUser = "A aparut o eroare";
                    return View("GetRegisterCode");
                }
                
                
            }
            
        }
        private async Task<bool> GetCode(string type)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var token = Request.Cookies["token3"];
                if (token != null)
                {
                    var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var responseToken = JsonSerializer.Deserialize<UserToken>(
                        token, jsonSerializerOptions);
                    if(responseToken != null)
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                        responseToken.Token);
                }
                var respuesta = await client.GetAsync($"{HomeController.url}/user/{type}?username={registerCredentials.UserName}&&email={registerCredentials.Email}");

                if (respuesta.IsSuccessStatusCode)
                    return true;
               return false;

            }
        }
        public async Task<ActionResult> ResendCode(string type)
        {

            var code = await GetCode(type);
            if(!code)
            {
                ViewBag.ErrorUser = "A aparut o eroare";
            }
            if (type.Equals("registercode"))
                return View("GetRegisterCode");
            else if (type.Equals("newpasscode"))
            {
                return View("GetNewPassCode");
            }
            else if(type.Equals("deletecode"))
            {
                return View("GetDeleteCode");
            }
            else return Content("inca lucrez la asta");
            
        }
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> GetRegisterCode(RegisterModel registerModel)
        {
            if(registerModel.Password!= null && !registerModel.Password.Equals(registerModel.RepeatPassword))
            {
                ViewBag.ErrorCreateUser = "The passwords are not the same";
                return View("CreateUser");
            }
            if(registerModel.Password != null && registerModel.Password.Length<8)
            {
                ViewBag.ErrorCreateUser = "Password too short";
                return View("CreateUser");
            }
            if (registerModel.Email != null && registerModel.UserName != null)
            {
                registerCredentials.UserName = registerModel.UserName;
                registerCredentials.Email = registerModel.Email;
                registerCredentials.Password = registerModel.Password;
            }
            else
            {
                ViewBag.ErrorCreateUser = "No null values allowed";
                return View("CreateUser");
            }
            var code = await GetCode("registercode");
            if(code == false)
            {
                ViewBag.ErrorCreateUser = "There is an account with this username/email or the password is too short";
                return View("CreateUser");
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> LogInAction(Credentials credentials, string rememberMe )
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                
                HttpResponseMessage httpResponseToken = await client.PostAsJsonAsync($"{HomeController.url}/user/login", credentials);
                if(httpResponseToken.StatusCode == HttpStatusCode.BadRequest)
                {
                    ViewBag.ErrorUser = "Invalid log in attempt";
                    return View("LogIn");
                }
                var stringtoken = await httpResponseToken.Content.ReadAsStringAsync();
                if (rememberMe != null)
                {
                    Response.Cookies.Append("token3", stringtoken, 
                        new CookieOptions() { Expires = DateTime.Now.AddDays(30) });
                }
                else
                {
                    Response.Cookies.Append("token3", stringtoken);
                }

                
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<ActionResult> AboutUser(Guid? id)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = new HttpResponseMessage();
                if (id == null)
                {
                    var token = Request.Cookies["token3"];
                    if (token != null)
                    {
                        var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                        var responseToken = JsonSerializer.Deserialize<UserToken>(
                            token, jsonSerializerOptions);
                        if(responseToken != null)
                        response = await client.GetAsync($"{HomeController.url}/user/{responseToken.UserId}/postscomments");
                    }
                }

                else
                {
                    response = await client.GetAsync($"{HomeController.url}/user/{id}/postscomments");
                }
                if (response.IsSuccessStatusCode)
                {
                    var user3 = await response.Content.ReadFromJsonAsync<UserPostsCommentsInfo>();
                    return View(user3);
                }
                return Content("nu merge...");
            }
        }
        public ActionResult LogIn()
        {
            
            return View();
        }
        public ActionResult LogOut()
        {
            HttpContext.Response.Cookies.Delete("token3");
            return RedirectToAction("Index","Home");
        }

        public async Task<ActionResult> Edit()
        {
            using (HttpClient client = new HttpClient())
            {
                var token = Request.Cookies["token3"];
                if (token != null)
                {
                    var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var responseToken = JsonSerializer.Deserialize<UserToken>(
                        token, jsonSerializerOptions);
                    if (responseToken != null)
                    {
                        HttpResponseMessage response = await client.GetAsync($"{HomeController.url}/user/{responseToken.UserId}");
                        if (response.IsSuccessStatusCode)
                        {
                            var user = await response.Content.ReadFromJsonAsync<UserInfo>();
                            if (user != null)
                            {
                                RegisterModel registerModel = new RegisterModel()
                                {
                                    Email = user.Email,
                                    UserName = user.UserName
                                };
                                return View(registerModel);
                            }
                        }
                    }
                }
                return Content("Not found");
            }
        }
        public async Task<ActionResult> GetNewPassCode(RegisterModel registerModel)
        {
            if (registerModel.Password!= null && !registerModel.Password.Equals(registerModel.RepeatPassword))
            {
                ViewBag.ErrorCreateUser = "The passwords are not the same";
                return View("Edit");
            }
            if (registerModel.UserName != null && registerModel.Email != null)
            {
                registerCredentials.UserName = registerModel.UserName;
                registerCredentials.Email = registerModel.Email;
                registerCredentials.Password = registerModel.Password;
            }
            else
            {
                ViewBag.ErrorCreateUser = "No null values allowed";
                return View("Edit");
            }
            var code = await GetCode("newpasscode");
            if (code == false)
            {
                ViewBag.ErrorCreateUser = "There is a problem";
                return View("Edit");
            }
            return View();
        }
        public async Task<ActionResult> ChangePassword(string code)
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
                var token = Request.Cookies["token3"];
                if (token != null)
                {
                    var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var responseToken = JsonSerializer.Deserialize<UserToken>(
                        token, jsonSerializerOptions);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (responseToken != null)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                            responseToken.Token);
                        var respueste = await client.PutAsJsonAsync($"{HomeController.url}/user", userCode);
                        if (respueste.IsSuccessStatusCode)
                        {
                            registerCredentials.Password = null;
                            return RedirectToAction("Index", "Home");
                        }
                    }

                }
                    ViewBag.ErrorUser = "A aparut o eroare";
                    return View("GetNewPassCode");
                
            }
        }
        public async Task<ActionResult> Delete()
        {
            using (HttpClient client = new HttpClient())
            {
                var token = Request.Cookies["token3"];
                if (token != null)
                {
                    var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var responseToken = JsonSerializer.Deserialize<UserToken>(
                        token, jsonSerializerOptions);
                    if (responseToken != null)
                    {
                        HttpResponseMessage response = await client.GetAsync($"{HomeController.url}/user/{responseToken.UserId}");
                        if (response.IsSuccessStatusCode)
                        {
                            var user = await response.Content.ReadFromJsonAsync<UserInfo>();
                            return View(user);
                        }
                    }
                }
                return Content("Not found");
            }
        }
        public async Task<ActionResult> GetDeleteCode(UserModel userModel)
        {
            
            registerCredentials.UserName = userModel.UserName;
            registerCredentials.Email = userModel.Email;
            var code = await GetCode("deletecode");
            if (code == false)
            {
                ViewBag.ErrorCreateUser = "There is a problem";
                return View("Delete");
            }
            return View();
        }
        public async Task<ActionResult> DeleteUser(string code)
        {
            ValidationCode coderead = new ValidationCode()
            {
                Code = code,
                Created = DateTime.Now
            };
            using (HttpClient client = new HttpClient())
            {
                var token = Request.Cookies["token3"];
                if (token != null)
                {
                    var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var responseToken = JsonSerializer.Deserialize<UserToken>(
                        token, jsonSerializerOptions);
                    if (responseToken != null)
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                            responseToken.Token);
                        var respueste = await client.DeleteAsync($"{HomeController.url}/user/delete?username={registerCredentials.UserName}&&codeFromUser={coderead.Code}&&created={coderead.Created.ToString()}");
                        if (respueste.StatusCode == HttpStatusCode.NoContent)
                        {
                            HttpContext.Response.Cookies.Delete("token3");
                            return RedirectToAction("Index", "User");

                        }
                    }
                }
                ViewBag.ErrorUser = "A aparut o eroare";
                return View("GetDeleteCode");
                
            }
        }

    }
}
