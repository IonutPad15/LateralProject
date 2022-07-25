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
        public static readonly string url = "http://localhost:5083/api";

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