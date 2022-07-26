using Microsoft.AspNetCore.Mvc;
using Models.Response;
using System.Net.Http.Headers;
using TheForestManMVC.Utils;
using Models.Request;
using Models.Response;

namespace TheForestManMVC.Controllers
{
    public class CommentController : Controller
    {
        private static Guid PostId;
        
        public IActionResult Index()
        {
            return View();
        }
        private async Task<PostInfo> GetAllComments(Guid id)
        {
            PostId= id;
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
        [HttpPost]
        public async Task<ActionResult> CreateComment(string isAnonyme, string commentBody)
        {
            
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                CommentRequest commentRequest = new CommentRequest()
                {
                    Body = commentBody,
                    PostId = PostId
                };
                if (UserController.token == null || UserController.token != null && isAnonyme != null)
                {
                    var requestComm = await client.PostAsJsonAsync($"{HomeController.url}/comment/loggedout", commentRequest);
                    
                }
                else
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                    UserController.token.Token);
                    var requestComm = await client.PostAsJsonAsync($"{HomeController.url}/comment/loggedin", commentRequest);
                }
            }
            return RedirectToAction("CommentsByPostId","Comment", new { id = PostId },null);
        }
        public async Task<ActionResult> EditComment(Guid id, string body)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                 UserController.token.Token);
                var requestComm = await client.PutAsJsonAsync($"{HomeController.url}/comment/{id}", body);

            }
            return Content("");
        }
        [HttpGet]
        public async Task<ActionResult> Edit(Guid id, string body)
        {
            CommentInfo comment = new CommentInfo()
            {
                Body = body,
                Id = id
            };
            return View(comment);
        }
    }
    
}
