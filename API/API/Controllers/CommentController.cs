﻿
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
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCommentById(Guid id)
        {
            var comment = await CommentService.GetCommentById(_context,id);
            if(comment == null) return NotFound(); 
            CommentInfo commentInfo = new CommentInfo()
            {
                Id = comment.Id,
                Created = comment.Created,
                Updated = comment.Updated,
                UserId = comment.UserId,
                Author = comment.Author,
                CommentBody = comment.CommentBody,
                PostId = comment.PostId

            };
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
                
                var user = await UserService.GetUserByUsername(_context, userclaim.Value);
                if (user != null)
                {
                    comment.CommentBody = commentRequest.Body;
                    comment.Author = user.UserName;
                    comment.UserId = user.Id;
                    comment.PostId = commentRequest.PostId;
                    comment.Updated = DateTime.Now;
                    comment.Created = DateTime.Now;
                    
                    var codeResult = await CommentService.CreateComment(_context, comment);
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
            
            var codeResult = await CommentService.CreateComment(_context, comment);
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
            
            var comment = await CommentService.GetCommentById(_context, id);
            if (comment == null) return NotFound();
            
            var user = UserService.GetUserById(_context, comment.UserId);
            if (user == null || !userclaim.Value.Equals(comment.Author))
            {
                return BadRequest("Not your comment");
            }
            comment.CommentBody = newBody;
            comment.Updated = DateTime.Now;

            var codeResult =await CommentService.UpdateComment(_context, comment);
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

            var commentToDelete = await CommentService.GetCommentById(_context, id);
            if (commentToDelete == null) return NotFound();
            if (commentToDelete.UserId == null)
            {
                return BadRequest("Anonymous comments cannot be deleted");
            }
            
            var user = await UserService.GetUserById(_context, commentToDelete.UserId);
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
            
            var codeResult = await CommentService.UpdateComment(_context, commentToDelete);
            if (codeResult == DbCodes.Codes.Error)
                return BadRequest("Something went wrong");

            return NoContent();
        }
    }
}

