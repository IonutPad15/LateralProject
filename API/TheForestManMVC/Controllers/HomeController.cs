using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TheForestManMVC.Models;
using Models.Request;
using Models.Response;
using System.Net.Http.Headers;

namespace TheForestManMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public static readonly string url = "https://localhost:7083/api";
        public static readonly HttpClient client = new HttpClient();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                    UserController.token.Token);
                PostRequest postRequest = new PostRequest()
                {
                    Body = postBody,
                    Title = postTitle
                };
                HttpResponseMessage response = await client.PostAsJsonAsync($"{url}/post", postRequest);
                return Content("");
            }
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