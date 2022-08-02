
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Models;
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
    public class CommentController : ControllerBase
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
        public CommentController(SiteDbContext context)
        {
            _context = context;
            mapper = new Mapper(config);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCommentById(Guid id)
        {
            CommentService commentService = new CommentService();
            var comment = await commentService.GetCommentById(_context,id);
            if(comment == null) return NotFound();
            CommentInfo commentInfo = mapper.Map<CommentInfo>(comment);
            return Ok(commentInfo);

        }
        
        [HttpPost("loggedin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateLoggedIn([FromBody] CommentRequest commentRequest)
        {
            Comment comment = new Comment();
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            if (userclaim != null)
            {
                UserService userService = new UserService();
                var user = await userService.GetUserByUsername(_context, userclaim.Value);
                if (user != null)
                {
                    comment.CommentBody = commentRequest.Body;
                    comment.Author = user.UserName;
                    comment.UserId = user.Id;
                    comment.PostId = commentRequest.PostId;
                    comment.Updated = DateTime.Now;
                    comment.Created = DateTime.Now;
                    CommentService commentService = new CommentService();
                    var codeResult = await commentService.CreateComment(_context, comment);
                    if (codeResult == DbCodes.Codes.Error)
                        return BadRequest("Something went wrong");
                    return Ok();
                }
            }
            return BadRequest();

        }

        [HttpPost("loggedout")]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAnonyme([FromBody] CommentRequest commentRequest)
        {
            
            Comment comment = new Comment();
            Random rnd = new Random();
            comment.Author = $"Anonymous{rnd.Next(99999)}";
            comment.CommentBody = commentRequest.Body;
            comment.PostId = commentRequest.PostId;
            comment.Updated = DateTime.Now;
            comment.Created = DateTime.Now;
            CommentService commentService = new CommentService();
            var codeResult = await commentService.CreateComment(_context, comment);
            if (codeResult == DbCodes.Codes.Error)
                return BadRequest("Something went wrong");
            return Ok();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Update(Guid id, [FromBody] string newBody)
        {
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            if (userclaim == null)
            {
                return BadRequest();
            }
            CommentService commentService = new CommentService();
            var comment = await commentService.GetCommentById(_context, id);
            if (comment == null) return NotFound();
            UserService userService = new UserService();
            var user = userService.GetUserById(_context, comment.UserId);
            if (user == null || !userclaim.Value.Equals(comment.Author))
            {
                return BadRequest("Not your comment");
            }
            comment.CommentBody = newBody;
            comment.Updated = DateTime.Now;

            var codeResult =await commentService.UpdateComment(_context, comment);
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
            CommentService commentService = new CommentService();
            var commentToDelete = await commentService.GetCommentById(_context, id);
            if (commentToDelete == null) return NotFound();
            if (commentToDelete.UserId == null)
            {
                return BadRequest("Anonymous comments cannot be deleted");
            }
            UserService userService = new UserService();
            var user = await userService.GetUserById(_context, commentToDelete.UserId);
            if (user == null)
            {
                return BadRequest("Anonymous comments cannot be editted");
            }
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            
            if (userclaim != null)
            {
                if (!userclaim.Value.Equals(user.UserName))
                    return BadRequest("Not his post");
            }
            commentToDelete.IsDeleted = true;
            
            var codeResult = await commentService.UpdateComment(_context, commentToDelete);
            if (codeResult == DbCodes.Codes.Error)
                return BadRequest("Something went wrong");

            return NoContent();
        }
    }
}

