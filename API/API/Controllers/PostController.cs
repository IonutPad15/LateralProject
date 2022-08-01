
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Models.Request;
using Models.Response;
using API.Services;
using AutoMapper;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly SiteDbContext _context;
        private readonly MapperConfiguration config = new MapperConfiguration(cfg => {
            cfg.CreateMap<User, UserInfo>();
            cfg.CreateMap<User, UserPostsCommentsInfo>();
            cfg.CreateMap<Post, PostInfo>();
            cfg.CreateMap<Comment, CommentInfo>();
            cfg.CreateMap<UserCode, User>();
        });
        private readonly Mapper mapper;
        public PostController(SiteDbContext context)
        {
            mapper = new Mapper(config);
            _context = context;
        }
        [HttpGet]
        public async Task<IEnumerable<PostInfo>> GetPosts([FromHeader] int postAmount = -1)
        {
            List<Post> posts = await PostService.GetPosts(_context);

            //var postsInfos = from post in posts
            //                select new PostInfo()
            //                {
            //                    Id = post.Id,
            //                    Author = post.Author,
            //                    Description = post.Description,
            //                    Created = post.Created,
            //                    Updated = post.Updated,
            //                    Title = post.Title,
            //                    UserId = post.UserId,

            //                };
            //List<PostInfo> postsInfo = postsInfos.ToList();
            //int i = 0;

            //foreach(Post post in posts)
            //{
            //    if(post.Comments != null)
            //    foreach(var comment in post.Comments)
            //    {
            //        if (comment.IsDeleted == false)
            //        {
            //            CommentInfo comm = new CommentInfo()
            //            {
            //                Id = comment.Id,
            //                Created = comment.Created,
            //                Updated = comment.Updated,
            //                Author = comment.Author,
            //                CommentBody = comment.CommentBody,
            //                UserId = comment.UserId
            //            };
            //            postsInfo[i].Comments.Add(comm);
            //        }
            //    }
            //    postsInfo[i].Comments= postsInfo[i].Comments.OrderByDescending(p => p.Updated).ToList<CommentInfo>();
            //    i++;
            //}
            var postInfos = mapper.Map<List<PostInfo>>(posts);
            if (postAmount < 0)
                return postInfos;

            List<PostInfo> nextposts = postInfos.Skip(Math.Max(0, postInfos.Count - postAmount)).ToList();
            return nextposts;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PostInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPostByid(Guid id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p=>p.Id==id && p.IsDeleted==false);
            if(post == null) return NotFound();
            PostInfo postInfo = new PostInfo()
            {
                Author = post.Author,
                Description = post.Description,
                Created = post.Created,
                Id = id,
                Title = post.Title,
                Updated = post.Updated,
                UserId = post.UserId
                

            };
            return  Ok(postInfo);

        }
        [HttpGet("{id}/comments")]
        [ProducesResponseType(typeof(PostInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCommentsByPostid(Guid id)
        { 
            var posttester = await PostService.GetPostWithCommentsByPostId(_context, id);
            if(posttester == null) return NotFound();
            
            PostInfo postcomments = mapper.Map<PostInfo>(posttester);
            return Ok(postcomments);

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
                var user = await UserService.GetUserByUsername(_context, userclaim.Value);
                if (user == null) return BadRequest();
                Post post = new Post();
                post.Title = postrequest.Title;
                post.Description = postrequest.Body;
                post.Author = user.UserName;
                post.UserId = user.Id;
                post.Created = DateTime.Now;
                post.Updated = DateTime.Now;
                var codeResult = await PostService.CreatePost(_context, post);
                if (codeResult == DbCodes.Codes.Error)
                    return BadRequest("Something went wrong");
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
            
            var post = await PostService.GetPostById(_context,id);
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
            var codeResult = await PostService.UpdatePost(_context, post);
            if (codeResult == DbCodes.Codes.Error)
                return BadRequest("Something went wrong");

            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var postToDelete = await PostService.GetPostById(_context, id);
            if (postToDelete == null) return NotFound();
            var user = await _context.Users.FirstOrDefaultAsync(u=> u.Id== postToDelete.UserId && u.IsDeleted ==false);
            if (user == null) return BadRequest("You can't delete this post");
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            if (userclaim != null)
            {
                if (!userclaim.Value.Equals(user.UserName))
                    return BadRequest("Not his post");
            }
            
            var posttester = await PostService.GetPostWithCommentsByPostId(_context, id);
            if(posttester == null) return NotFound();
            if(posttester.Comments != null)
            for(int i=0; i<posttester.Comments.Count;++i)
            {
                var comment = posttester.Comments[i];
                comment.IsDeleted = true;
                _context.Entry(comment).State = EntityState.Modified;
            }
            postToDelete.IsDeleted = true;
            var codeResult = await PostService.UpdatePost(_context, postToDelete);
            if (codeResult == DbCodes.Codes.Error)
                return BadRequest("Something went wrong");

            return NoContent();
        }
    }
}