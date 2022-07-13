using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;


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
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = comment.Id }, comment);
        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, Comment comment)
        {
            if (id != comment.Id) return BadRequest();
            _context.Entry(comment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var commentToDelete = await _context.Posts.FindAsync(id);
            if (commentToDelete == null) return NotFound();

            _context.Posts.Remove(commentToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

