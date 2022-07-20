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
            return await _context.Comments.ToListAsync();
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
        

        [HttpPost]
        [ProducesResponseType(typeof(Comment), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(Comment comment)
        {
            if (comment.UserId == null)
            {

                Random rnd = new Random();
                comment.Author = $"Anonymous{rnd.Next(99999)}";
            }
            else
            {
                var user = await _context.Users.FindAsync(comment.UserId);
                comment.Author = user.UserName;
            }
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
            var user = await _context.Users.FindAsync(comment.UserId);
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
        public async Task<IActionResult> Delete(int id)
        {
           
            var commentToDelete = await _context.Posts.FindAsync(id);
            if (commentToDelete == null) return NotFound();
            if (commentToDelete.UserId == null)
            {
                return BadRequest("Anonymous comments cannot be deleted");
            }
            var user = await _context.Users.FindAsync(commentToDelete.UserId);
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            
            if (userclaim != null)
            {
                if (!userclaim.Value.Equals(user.UserName))
                    return BadRequest("Not his post");
            }
            
            if (commentToDelete == null) return NotFound();

            _context.Posts.Remove(commentToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

