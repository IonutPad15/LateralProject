using Microsoft.AspNetCore.Mvc;
using Models.Response;
using System.Net.Http.Headers;
using TheForestManMVC.Utils;

namespace TheForestManMVC.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private async Task<PostInfo> GetAllComments(Guid id)
        {
            PostInfo postInfo = new PostInfo();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync($"{HomeController.url}/post/{id}/comments");
                if (response.IsSuccessStatusCode)
                {
                    var post = await response.Content.ReadFromJsonAsync<PostInfo>();

                    return post;
                }
            }
            return postInfo;
        }
        public async Task<ActionResult> CommentsByPostId( Guid id)
        {
            PostInfo post = await GetAllComments(id);
            return View(post);
        }
    }
}
