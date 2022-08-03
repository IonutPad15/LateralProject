﻿
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Models.Request;
using Models.Response;
using API.Services;
using AutoMapper;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static Dictionary<string, ValidationCode> userCodes= new Dictionary<string, ValidationCode>();
        private readonly SiteDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly MapperConfiguration config = new MapperConfiguration(cfg => {
            cfg.CreateMap<User, UserInfo>();
            cfg.CreateMap<User, UserPostsCommentsInfo>();
            cfg.CreateMap<Post, PostInfo>().ForMember(
                dest => dest.Votes, opt => opt.MapFrom(src => CalculateVotes(src.Votes)));
            cfg.CreateMap<Comment, CommentInfo>().ForMember(
                dest => dest.Votes, opt => opt.MapFrom(src => CalculateVotes(src.Votes)));
            cfg.CreateMap<UserCode, User>();
            cfg.CreateMap<VoteRequest, Vote>();
        });
        private readonly Mapper mapper;
        public UserController(SiteDbContext context, IConfiguration configuration)
        {
            _context = context;
            mapper = new Mapper(config);
            _configuration = configuration;

        }
        private static int CalculateVotes(List<Vote> votes)
        {
            return votes.Where(v => v.IsUpVote == true).Count()
                - votes.Where(v => v.IsUpVote == false).Count();
        }

        [HttpGet]
        public async Task<IEnumerable<UserInfo>> GetUsers()
        {
            UserService userService = new UserService();
            List<UserInfo> userInfos = await userService.GetUsers(_context);
                return userInfos;
            
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(Guid id)
        {
            UserService userService = new UserService();
            var user = await userService.GetUserById(_context, id);
            if (user == null) return NotFound();
            UserInfo userInfo = mapper.Map<UserInfo>(user);
            return Ok(userInfo);
        }

        [HttpGet("{id}/postscomments")]
        [ProducesResponseType(typeof(UserPostsCommentsInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPostsAndCommentsByUserId(Guid id)
        {
            UserService userService = new UserService();
            var usertester = await userService.GetUserWithPostsAndCommentsByUserId(_context, id);
            if(usertester == null) return NotFound();
            UserPostsCommentsInfo userpostinfo = mapper.Map<UserPostsCommentsInfo>(usertester);
            return  Ok(userpostinfo);

        }

        private enum ResultCode
        {
            InvalidAdress, ValidAdress, Error
        }
        private ResultCode SendCode(string username, string email, string message)
        {
            Sender sender = new Sender();
            string verif = sender.SendEmail(email, message);
            if (!verif.Equals(ResultCode.Error.ToString()) && !verif.Equals(ResultCode.InvalidAdress.ToString()))
            {
                ValidationCode code = new ValidationCode()
                {
                    Code = verif,
                    Created = DateTime.Now
                };
                if (userCodes.ContainsKey(username))
                    userCodes[username] = code;
                else userCodes.Add(username, code);
                return ResultCode.ValidAdress;
            }
            if(verif.Equals(ResultCode.InvalidAdress.ToString()))   return ResultCode.InvalidAdress;
            return ResultCode.Error;
        }

        [HttpGet("registercode")]
        public async Task<ActionResult> GetRegisterCode([FromQuery] string username,[FromQuery] string email)
        {
            if (username == null || email == null) return BadRequest("username and email are required");
            UserService userService = new UserService();
            var usernameExists = await userService.GetUserByUsername(_context, username);
            if (usernameExists == null)
            {

                if (SendCode(username, email, "You've made an account on TheForestMan! ") == ResultCode.ValidAdress)
                    return Ok(username);
                else
                    return BadRequest("Eroare");
            }
            else
                return BadRequest("Already exists");

        }
        
        [HttpPost("create")]
        [ProducesResponseType(typeof(UserCode), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserCode userCode)
        {
            if (userCode.UserName == null 
                || userCode.Email == null
                || userCode.Password == null
                || userCode.Code == null
                || userCode.Code.Code == null) return BadRequest("All fields required");
            if (userCode.Password.Length < 8) return BadRequest("Password too short");
            if (userCodes.ContainsKey(userCode.UserName))
            {
                ValidationCode code = userCodes[userCode.UserName];
                
                TimeSpan diff = userCode.Code.Created.Subtract(code.Created);
                
                if(diff.TotalSeconds>60) return BadRequest("Too late");
                
                else if(code.Code.Equals(userCode.Code.Code))
                {
                    HashHelper hashHelper = new HashHelper();
                    string hashedPassword = hashHelper.GetHash(userCode.Password);
                    userCode.Password = hashedPassword;
                    User user = mapper.Map<User>(userCode);
                    UserService userService = new UserService();
                    var codeResult = await userService.CreateUser(_context, user);
                    if (codeResult == DbCodes.Codes.Error) 
                        return BadRequest("ERROR");
                    return BuildToken(user);
                }
                else return BadRequest("Code not valid");
                
            }
            else return BadRequest("Eroare");

        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("newpasscode")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNewPassCode([FromQuery] string username, [FromQuery] string email)
        {
            UserService userService = new UserService();
            var userExists = await userService.GetUserByUsernameAndEmail(_context, username, email);
            if (userExists != null)
            {
                var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
                var emailclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email));
                if (userclaim != null && emailclaim!=null)
                {

                    if (!userclaim.Value.Equals(userExists.UserName) || !emailclaim.Value.Equals(userExists.Email))
                        return BadRequest("Not his account");
                }
                if (SendCode(username, email, "You want to change your password! ") == ResultCode.ValidAdress)
                    return Ok(username);
                else return BadRequest("Eroare");
            }
            else return BadRequest("User not found");
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] UserCode userCode)
        {
            UserService userService = new UserService();
            var userToUpdate = await userService.GetUserByUsernameAndEmail(_context, 
                userCode.UserName, userCode.Email);
            if (userToUpdate == null) return NotFound();
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            var emailclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email));

            if (userclaim != null && emailclaim!=null)
            {
                if (!userclaim.Value.Equals(userToUpdate.UserName) || !emailclaim.Value.Equals(userToUpdate.Email))
                    return BadRequest("Not his account");
                if (userToUpdate == null) return NotFound();
                HashHelper hashHelper = new HashHelper();
                string hashedPassword = hashHelper.GetHash(userCode.Password);
                userToUpdate.Password = hashedPassword;
                
                var codeResult = await userService.UpdateUser(_context, userToUpdate);
                if (codeResult == DbCodes.Codes.Error) return BadRequest("Something went wrong");
                return Ok();
            }
            return NoContent();
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("deletecode")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDeleteCode([FromQuery] string username, [FromQuery] string email)
        {
            UserService userService = new UserService();
            var userExists = await userService.GetUserByUsernameAndEmail(_context,username, email);
            if (userExists != null)
            {
                var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
                var emailclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email));

                if (userclaim != null && emailclaim!=null)
                {

                    if (!userclaim.Value.Equals(userExists.UserName) || !emailclaim.Value.Equals(userExists.Email))
                        return BadRequest("Not his account");
                }
                if (SendCode(username, email, "You want to delete your account! ") == ResultCode.ValidAdress)
                    return Ok(username);
                else return BadRequest("Eroare");
            }
            else
            {
                return BadRequest("User not found");
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser([FromQuery] string username,
            [FromQuery]string codeFromUser,
            [FromQuery] string created
            )
        {
            UserService userService = new UserService();
            var userToDelete = await userService.GetUserByUsername(_context, username);
            if (userToDelete == null) return NotFound();
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            if (userclaim != null)
            {
                if (userCodes.ContainsKey(username))
                    if (!userclaim.Value.Equals(userToDelete.UserName))
                    return BadRequest("Not his account");
                
                {
                    ValidationCode code = userCodes[username];
                    DateTime dateTime = DateTime.Parse(created);
                    TimeSpan diff = dateTime.Subtract(code.Created);

                    if (diff.TotalSeconds > 60)
                    {
                        return BadRequest("Too late");
                    }
                    else if(code.Code.Equals(codeFromUser))
                    {
                        var usertester = await userService.GetUserWithPostsAndCommentsByUserId(_context, userToDelete.Id);
                        if (usertester == null) return NotFound();
                        if (usertester.Posts != null)
                        {
                            for (int i = 0; i < usertester.Posts.Count; ++i)
                            {
                                var post = usertester.Posts[i];
                                post.Author = "[User Deleted]";
                                
                                post.Updated = DateTime.Now;
                                _context.Entry(post).State = EntityState.Modified;
                            }
                        }
                        if (usertester.Comments != null)
                        {
                            for (int i = 0; i < usertester.Comments.Count; ++i)
                            {
                                var comment = usertester.Comments[i];
                                comment.Author = "[User Deleted]";
                                comment.Updated = DateTime.Now;
                                _context.Entry(comment).State = EntityState.Modified;
                            }
                        }
                        userToDelete.IsDeleted = true;
                        var codeResult = await userService.UpdateUser(_context, userToDelete);
                        if (codeResult == DbCodes.Codes.Error)
                            return BadRequest("Something went wrong");
                        return NoContent();
                    }
                    else return BadRequest("Code not valid");
                }
            }
            return BadRequest("Eroare");
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] Credentials credentials)
        {
            UserService userService = new UserService();
            HashHelper hashHelper = new HashHelper();
            string hashedPassword = hashHelper.GetHash(credentials.Password);
            credentials.Password = hashedPassword;
            var user = await userService.GetUserByCredentials(_context, credentials);
            if (user == null) return BadRequest("Invalid login attempt");
            else return BuildToken(user);
        }
        
        private UserToken BuildToken(User user)
        {
            var expiration = DateTime.Now.AddDays(30);
            if (user.UserName != null && user.Email != null)
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTkey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                JwtSecurityToken token = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: claims,
                    expires: expiration,
                    signingCredentials: creds);

                return new UserToken()
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpirationDate = expiration,
                    UserId = user.Id
                };
            }
            return new UserToken();
        }
        [HttpPost("vote")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Vote([FromBody] VoteRequest voteRequest)
        {

            ///de verificat toate cazurile
            VoteService voteService = new VoteService();
            UserService userService = new UserService();
            Vote? vote = new Vote();
            DbCodes.Codes codeResult = new DbCodes.Codes();
            if (voteRequest == null) return BadRequest();
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            if (userclaim == null) return Unauthorized();
            if(voteRequest.UserId == null)
            {
                //var user =await userService.GetUserByUsername(_context, userclaim.Value);
                //if (user != null)
                //{
                //    voteRequest.UserId = user.Id;
                //    vote = mapper.Map<Vote>(voteRequest);
                //    codeResult = await voteService.CreateVote(_context, vote);
                //}
                return Unauthorized();
            }
            else
            {
                var user = await userService.GetUserById(_context, voteRequest.UserId);
                if(user != null)
                {
                    if (!userclaim.Value.Equals(user.UserName))
                    {
                        return BadRequest();
                    }
                    if (voteRequest.PostId != null && voteRequest.CommentId == null)
                    {
                        vote = await voteService.GetVoteByUserAndPostId(_context, voteRequest.UserId, voteRequest.PostId);
                        if (vote != null)
                        {
                            if (vote.IsUpVote == voteRequest.IsUpVote)
                            {
                                codeResult = await voteService.DeleteVote(_context, vote);
                            }
                            else
                            {
                                vote.IsUpVote = voteRequest.IsUpVote;
                                codeResult = await voteService.UpdateVote(_context, vote);
                            }

                        }
                        else
                        {
                            vote = mapper.Map<Vote>(voteRequest);
                            codeResult = await voteService.CreateVote(_context, vote);
                        }
                    }
                    else
                    if (voteRequest.PostId == null && voteRequest.CommentId != null)
                    {
                        vote = await voteService.GetVoteByUserAndCommentId(_context, voteRequest.UserId, voteRequest.CommentId);
                        if (vote != null)
                        {
                            if (vote.IsUpVote == voteRequest.IsUpVote)
                            {
                                codeResult = await voteService.DeleteVote(_context, vote);
                            }
                            else
                            {
                                vote.IsUpVote = voteRequest.IsUpVote;
                                codeResult = await voteService.UpdateVote(_context, vote);
                            }

                        }
                        else
                        {
                            vote = mapper.Map<Vote>(voteRequest);
                            codeResult = await voteService.CreateVote(_context, vote);
                        }
                    }
                    else return BadRequest();
                }
            }
            if (codeResult == DbCodes.Codes.Succes)
            {
                return Ok();
            }
            else return UnprocessableEntity();
            
        }
    }
}
