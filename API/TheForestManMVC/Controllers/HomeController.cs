using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TheForestManMVC.Models;
using Models.Request;
using Models.Response;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Net;

namespace TheForestManMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public static readonly string url = "https://localhost:7083/api";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        private async Task<List<PostInfo>> GetAllPosts()
        {
            List <PostInfo> postsInfo= new List<PostInfo>();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                HttpResponseMessage response = await client.GetAsync($"{url}/post");
                if (response.IsSuccessStatusCode)
                {
                    var posts = await response.Content.ReadFromJsonAsync<IEnumerable<PostInfo>>();
                    
                    return posts.ToList();
                }
            }
            return postsInfo;
        }
        public async Task<IActionResult> Index()
        {
            List<PostInfo> postsInfo = await GetAllPosts();
            return View(postsInfo);
        }
        [HttpPost]
        public async Task<ActionResult> CreatePost(string postTitle, string postBody)
        {
            using(HttpClient client = new HttpClient())
            {
                var token = Request.Cookies["token3"];
                if (token != null)
                {
                    var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var responseToken = JsonSerializer.Deserialize<UserToken>(
                        token, jsonSerializerOptions);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                        responseToken.Token);
                    PostRequest postRequest = new PostRequest()
                    {
                        Body = postBody,
                        Title = postTitle
                    };
                    HttpResponseMessage response = await client.PostAsJsonAsync($"{url}/post", postRequest);
                }
                return RedirectToAction("Index");
                
            }
        }
        public async Task<ActionResult> EditPost(Guid id, string body)
        {
            using (HttpClient client = new HttpClient())
            {
                var token = Request.Cookies["token3"];
                if (token != null)
                {
                    var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var responseToken = JsonSerializer.Deserialize<UserToken>(
                        token, jsonSerializerOptions);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                        responseToken.Token);

                    HttpResponseMessage response = await client.PutAsJsonAsync($"{url}/post/{id}", body);
                }
                return RedirectToAction("Index");
            }
        }
        public ActionResult Edit(Guid id, string body)
        {
            PostInfo post = new PostInfo()
            {
                Id = id,
                Body = body
            };
            return View(post);
        }
        public async Task<ActionResult> DeletePost(PostInfo postInfo)
        {
            using (HttpClient client = new HttpClient())
            {
                var token = Request.Cookies["token3"];
                if (token != null)
                {
                    var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var responseToken = JsonSerializer.Deserialize<UserToken>(
                        token, jsonSerializerOptions);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                        responseToken.Token);
                    HttpResponseMessage postResponse = await client.DeleteAsync($"{HomeController.url}/post/{postInfo.Id}");

                    if (postResponse.StatusCode == HttpStatusCode.NoContent)
                    {

                        return RedirectToAction("Index");
                    }
                }
                return Content("Something went wrong");
            }
        }
        public async Task<ActionResult> Delete(Guid id)
        {
            using (HttpClient client = new HttpClient())
            {
                var token = Request.Cookies["token3"];
                if (token != null)
                {
                    var jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var responseToken = JsonSerializer.Deserialize<UserToken>(
                        token, jsonSerializerOptions);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                        responseToken.Token);
                    HttpResponseMessage postResponse = await client.GetAsync($"{HomeController.url}/post/{id}");

                    if (postResponse.IsSuccessStatusCode)
                    {

                        var response = await postResponse.Content.ReadAsStringAsync();
                        PostInfo postInfo = JsonSerializer.Deserialize<PostInfo>(response,
                                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                        return View(postInfo);
                    }
                }
            }
            return View();
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}