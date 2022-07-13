
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IEnumerable<Post>> Get()
        {
            return await _context.Posts.ToListAsync();
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Post), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByid(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            return post == null ? NotFound() : Ok(post);

        }


        [HttpPost]
        [ProducesResponseType(typeof(Post), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByid), new { id = post.Id }, post);
        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, Post post)
        {
            if (id != post.Id) return BadRequest();
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var postToDelete = await _context.Posts.FindAsync(id);
            if (postToDelete == null) return NotFound();

            _context.Posts.Remove(postToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}