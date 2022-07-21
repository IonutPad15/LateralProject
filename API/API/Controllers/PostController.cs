
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        /*[HttpGet]
        public async Task<IEnumerable<Post>> Get()
        {
            return await _context.Posts.ToListAsync();
        }*/
        [HttpGet]
        public async Task<IEnumerable<Post>> Get([FromHeader] int postAmount = -1)
        {
            List<Post> posts = await _context.Posts.ToListAsync();
            posts.RemoveAll(p => p.IsDeleted == true);
            if (postAmount < 0)
                return posts;

            List<Post> nextposts = posts.Skip(Math.Max(0, posts.Count - postAmount)).ToList();
            return nextposts;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Post), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByid(Guid id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p=>p.Id==id && p.IsDeleted==false);
            return post == null ? NotFound() : Ok(post);

        }
        [HttpGet("{id}/comments")]
        [ProducesResponseType(typeof(UserPostsInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCommentsByid(Guid id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id && p.IsDeleted == false);
            var posttester = _context.Posts.Include(x => x.Comments).Single(x => x.Id == id&&x.IsDeleted==false);
            return post == null ? NotFound() : Ok(posttester);

        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(Post), StatusCodes.Status200OK)]
        public async Task<Post> Create(Post post)
        {
            //ia userul din claim
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            
            if (userclaim != null)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userclaim.Value && u.IsDeleted == false);
                post.Author = user.UserName;
                post.UserId = user.Id;

            }
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }


        [HttpPut("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Update(Guid id, Post post)
        
        {
            var user = await _context.Users.FirstOrDefaultAsync(u=>u.Id ==post.UserId && u.IsDeleted==false);
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            if(userclaim !=null)
            {
                if(!userclaim.Value.Equals(user.UserName)) 
                    return BadRequest("Not his post");
            }
            if (id != post.Id) 
                return BadRequest("Not good");
            post.Updated = DateTime.Now;
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
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