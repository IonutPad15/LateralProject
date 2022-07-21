
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Models.Request;
using Models.Response;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly SiteDbContext _context;
        public CommentController(SiteDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IEnumerable<CommentInfo>> GetComments()
        {
            //List<Comment> comments = await _context.Comments.ToListAsync();
            // comments.RemoveAll(c => c.IsDeleted == true);
            //List<CommentInfo> commentsinfo= new List<CommentInfo>();
            //foreach (Comment comment in comments)
            //{
            //    CommentInfo commentInfo = new CommentInfo()
            //    {
            //        Created = comment.Created,
            //        Id=comment.Id,
            //        Updated = comment.Updated,
            //        Author = comment.Author,
            //        Body = comment.CommentBody
            //    };
            //    commentsinfo.Add(commentInfo);
            //}
            //return commentsinfo;
            var comments = from comment in _context.Comments
                           where comment.IsDeleted == false
                           select new CommentInfo()
                           {
                               Created = comment.Created,
                               Id = comment.Id,
                               Updated = comment.Updated,
                               Author = comment.Author,
                               Body = comment.CommentBody
                           };
            return comments;
            List<CommentInfo> commentInfos = await comments.ToListAsync<CommentInfo>();

            return commentInfos;
        }
        
        [HttpPost("loggedin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateLoggedIn([FromBody]String body)
        {
            Comment comment = new Comment();
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            CommentInfo commentInfo = new CommentInfo();
            if (userclaim != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userclaim.Value && u.IsDeleted == false);
                comment.CommentBody = commentInfo.Body = body;
                comment.Author = commentInfo.Author = user.UserName;
                comment.UserId = user.Id;
                comment.Updated = commentInfo.Updated = DateTime.Now;
                comment.Created = commentInfo.Created = DateTime.Now;
                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();
                return Ok(commentInfo);
            }
            else
            {
                return BadRequest();
            }

        }
        [HttpPost("loggedout")]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAnonyme([FromBody] string body)
        {
            CommentInfo commentInfo = new CommentInfo();
            Comment comment = new Comment();
            Random rnd = new Random();
            comment.Author =commentInfo.Author = $"Anonymous{rnd.Next(99999)}";
            comment.CommentBody = body;
            comment.Updated = commentInfo.Updated = DateTime.Now;
            comment.Created = commentInfo.Created = DateTime.Now;
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
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
            var comment = _context.Comments.FirstOrDefault(c => c.Id == id && c.IsDeleted == false);
            if (comment == null) return NotFound();
            var user = await _context.Users.FirstOrDefaultAsync(u=> u.Id ==comment.UserId && u.IsDeleted==false
            &&userclaim.Value.Equals(comment.Author)
            );
            if(user==null)
            {
                return BadRequest("Not your comment");
            }
            
            
            //if (userclaim != null)
            //{
            //    if (!userclaim.Value.Equals(user.UserName))
            //        return BadRequest("Not his post");
            //}
            comment.CommentBody = newBody;
            comment.Updated = DateTime.Now;
            CommentInfo commentInfo = new CommentInfo()
            {
                Id = comment.Id,
                Created = comment.Created,
                Updated = comment.Updated,
                Body = comment.CommentBody,
                Author = comment.Author

            };
            _context.Entry(comment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(commentInfo);
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(Guid id)
        {

            var commentToDelete = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);
            if (commentToDelete == null) return NotFound();
            if (commentToDelete.UserId == null)
            {
                return BadRequest("Anonymous comments cannot be deleted");
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == commentToDelete.UserId && u.IsDeleted == false);
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
            
            if (commentToDelete == null) return NotFound();
            commentToDelete.IsDeleted = true;
            _context.Entry(commentToDelete).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

