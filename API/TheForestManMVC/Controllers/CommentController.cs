using Microsoft.AspNetCore.Mvc;
using Models.Response;
using System.Net.Http.Headers;
using TheForestManMVC.Utils;
using Models.Request;
using Models.Response;
using System.Text.Json;
using System.Net;

namespace TheForestManMVC.Controllers
{
    public class CommentController : Controller
    {
        private static Guid postId;
        private static Guid commentId;
        public IActionResult Index()
        {
            return View();
        }
        private async Task<PostInfo> GetAllComments(Guid id)
        {
            postId= id;
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
                    PostId = postId
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
            return RedirectToAction("CommentsByPostId","Comment", new { id = postId },null);
        }
        public async Task<ActionResult> EditComment(Guid id, string body, Guid? postId, Guid? userId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                 UserController.token.Token);
                var requestComm = await client.PutAsJsonAsync($"{HomeController.url}/comment/{id}", body);
                if(requestComm != null)
                {
                    
                }
            }
            if(userId == null)
            return RedirectToAction("CommentsByPostId", "Comment", new { id = postId }, null);
            else return RedirectToAction("AboutUser", "User", new { id = postId }, null);
        }
        [HttpGet]
        public async Task<ActionResult> Edit(Guid id, string body,Guid? postId, Guid? userId)
        {
            commentId = id;
            Guid nr = new Guid();
            CommentInfo comment = new CommentInfo()
            {
                Body = body,
                Id = id,
                PostId = postId,
                UserId = userId
            };
            return View(comment);
        }
        public async Task<ActionResult> DeleteComment(CommentInfo comment)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                 UserController.token.Token);
                HttpResponseMessage commentResponse = await client.DeleteAsync($"{HomeController.url}/comment/{commentId}");

                if (commentResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    
                }
            }
            return RedirectToAction("CommentsByPostId", "Comment", new { id = comment.PostId }, null);
        }
        public async Task<ActionResult> Delete(Guid id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer",
                 UserController.token.Token);
                HttpResponseMessage commentResponse =await client.GetAsync($"{HomeController.url}/comment/{id}");
                
                if(commentResponse.IsSuccessStatusCode)
                {
                    
                    var response = await commentResponse.Content.ReadAsStringAsync();
                    CommentInfo commentInfo = JsonSerializer.Deserialize<CommentInfo>(response,
                            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    commentId = id;
                    return View(commentInfo);
                }
            }
            return View();
        }
        
    }
    
}
