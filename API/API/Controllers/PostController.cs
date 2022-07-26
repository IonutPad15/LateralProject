
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Models.Request;
using Models.Response;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly SiteDbContext _context;
        public PostController(SiteDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IEnumerable<PostInfo>> GetPosts([FromHeader] int postAmount = -1)
        {
            //List<Post> posts = await _context.Posts.ToListAsync();
            //posts.RemoveAll(p => p.IsDeleted == true);
            List<Post> posts = await _context.Posts.Include(x => x.Comments ).ToListAsync();
            posts.RemoveAll(p => p.IsDeleted == true);
            var postinfos = from post in _context.Posts
                            where post.IsDeleted == false
                            select new PostInfo()
                            {
                                Id = post.Id,
                                Author = post.Author,
                                Body = post.Description,
                                Created = post.Created,
                                Updated = post.Updated,
                                Title = post.Title,
                                UserId = post.UserId,
                                
                            };
            int i = 0;
            
            List<PostInfo> postsInfo = await postinfos.ToListAsync<PostInfo>();
            foreach(Post post in posts)
            {
                foreach(var comment in post.Comments)
                {
                    if (comment.IsDeleted == false)
                    {
                        CommentInfo comm = new CommentInfo()
                        {
                            Id = comment.Id,
                            Created = comment.Created,
                            Updated = comment.Updated,
                            Author = comment.Author,
                            Body = comment.CommentBody,
                            UserId = comment.UserId
                        };
                        postsInfo[i].Comments.Add(comm);
                    }
                }
                postsInfo[i].Comments= postsInfo[i].Comments.OrderBy(p => p.Updated).ToList<CommentInfo>();
                postsInfo[i].Comments.Reverse();
                i++;
            }
            postsInfo = postsInfo.OrderBy(x => x.Updated).ToList<PostInfo>();
            postsInfo.Reverse();
            //List<PostInfo> posts = await postinfos.ToListAsync<PostInfo>();
            if (postAmount < 0)
                return postsInfo;

            List<PostInfo> nextposts = postsInfo.Skip(Math.Max(0, postsInfo.Count - postAmount)).ToList();
            return nextposts;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PostInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPostByid(Guid id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p=>p.Id==id && p.IsDeleted==false);
            PostInfo postInfo = new PostInfo()
            {
                Author = post.Author,
                Body = post.Description,
                Created = post.Created,
                Id = id,
                Title = post.Title,
                Updated = post.Updated,
                UserId = post.UserId
                

            };
            return post == null ? NotFound() : Ok(postInfo);

        }
        [HttpGet("{id}/comments")]
        [ProducesResponseType(typeof(PostInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCommentsByid(Guid id)
        {
            //var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
            var posttester = _context.Posts.Include(x => x.Comments).Single(x => x.Id == id && x.IsDeleted == false);
            posttester.Comments.RemoveAll(c => c.IsDeleted == true);
            PostInfo postcomments = new PostInfo()
            {
                Author = posttester.Author,
                Id = posttester.Id,
                Body = posttester.Description,
                Created = posttester.Created,
                Title = posttester.Title,
                Updated = posttester.Updated,
                UserId =posttester.UserId
            };
                foreach (var comment in posttester.Comments)
                {
                    CommentInfo commentinfo = new CommentInfo()
                    {
                        Author = comment.Author,
                        Body = comment.CommentBody,
                        Created = comment.Created,
                        Updated = comment.Updated,
                        Id = comment.Id,
                        UserId = comment.UserId,
                        PostId = comment.PostId
                    };
                
                    postcomments.Comments.Add(commentinfo);
                }
            postcomments.Comments = postcomments.Comments.OrderBy(c => c.Updated).ToList<CommentInfo>();
            postcomments.Comments.Reverse();
            return posttester == null ? NotFound() : Ok(postcomments);

        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(Post), StatusCodes.Status200OK)]
        public async Task<ActionResult<Post>> CreatePost(PostRequest postrequest)
        {
            //ia userul din claim
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            
            if (userclaim != null)
            {

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userclaim.Value && u.IsDeleted == false);
                if (user == null) return BadRequest();
                Post post = new Post();
                post.Title = postrequest.Title;
                post.Description = postrequest.Body;
                post.Author = user.UserName;
                post.UserId = user.Id;
                post.Created = DateTime.Now;
                post.Updated = DateTime.Now;
                await _context.Posts.AddAsync(post);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest();
        }


        [HttpPut("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdatePost(Guid id,[FromBody] string newDesciption)
        {
            //var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false 
            //&&p.User.UserName == userclaim.Value
            );
            if (post == null) return NotFound();
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == post.Author && u.IsDeleted == false);
            if (user == null) return NotFound("User");
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            if (userclaim != null)
            {
                if (!userclaim.Value.Equals(user.UserName))
                    return BadRequest("Not his post");
            }

            post.Description = newDesciption;
            post.Updated = DateTime.Now;
            _context.Entry(post).State = EntityState.Modified;
            PostInfo postInfo = new PostInfo()
            {
                Created = post.Created,
                Updated = post.Updated,
                Body = post.Description,
                Author = post.Author,
                Id = post.Id,
                Title = post.Title

            };
            await _context.SaveChangesAsync();

            return Ok();
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var postToDelete = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
            if (postToDelete == null) return NotFound();
            var user = await _context.Users.FirstOrDefaultAsync(u=> u.Id== postToDelete.UserId && u.IsDeleted ==false);
            if (user == null) return BadRequest("You can't delete this post");
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            if (userclaim != null)
            {
                if (!userclaim.Value.Equals(user.UserName))
                    return BadRequest("Not his post");
            }
            var posttester = _context.Posts.Include(x => x.Comments).Single(x => x.Id == id && x.IsDeleted == false);
            for(int i=0; i<posttester.Comments.Count;++i)
            {
                var comment = posttester.Comments[i];
                comment.IsDeleted = true;
                _context.Entry(comment).State = EntityState.Modified;
            }
            postToDelete.IsDeleted = true;
            _context.Entry(postToDelete).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}