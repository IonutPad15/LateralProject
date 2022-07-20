using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using NUnit.Framework;

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
        public async Task<IEnumerable<Comment>> Get()
        {
            List<Comment> comments = await _context.Comments.ToListAsync();
             comments.RemoveAll(c => c.IsDeleted == true);
            return comments;
        }

        /*
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByid(int id)
        {
            //TODO: do you search for comments or posts?
            //I don't...
            var comment = await _context.Posts.FindAsync(id);
            return comment == null ? NotFound() : Ok(comment);

        }*/
        

        [HttpPost("loggedin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateLoggedIn(Comment comment)
        {
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));

            if (userclaim != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userclaim.Value && u.IsDeleted == false);
                comment.Author = user.UserName;
                comment.UserId = user.Id;
                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Get), new { id = comment.Id }, comment);
            }
            else
            {
                return BadRequest();
            }
            
        }
        [HttpPost("loggedout")]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAnonyme(Comment comment)
        {


            Random rnd = new Random();
            comment.Author = $"Anonymous{rnd.Next(99999)}";
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = comment.Id }, comment);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Update(Guid id, Comment comment)
        {
            if (comment.UserId == null)
            {
                return BadRequest("Anonymous comments cannot be editted");
            }
            var user = await _context.Users.FirstOrDefaultAsync(u=> u.Id ==comment.UserId && u.IsDeleted==false);
            if(user==null)
            {
                return BadRequest("Anonymous comments cannot be editted");
            }
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            
            if (userclaim != null)
            {
                if (!userclaim.Value.Equals(user.UserName))
                    return BadRequest("Not his post");
            }
            if (id != comment.Id) return BadRequest();
            comment.Updated = DateTime.Now;
            _context.Entry(comment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(Guid id)
        {

            var commentToDelete = await _context.Posts.FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted == false);
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

