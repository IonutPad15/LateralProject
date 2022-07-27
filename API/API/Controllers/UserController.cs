using Microsoft.AspNetCore.Http;//TODO: remove unused namespaces
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

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static Dictionary<string, ValidationCode> userCodes= new Dictionary<string, ValidationCode>();
        private readonly SiteDbContext _context;
        private readonly IConfiguration _configuration;
        public UserController(SiteDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        [HttpGet]
        public async Task<IEnumerable<UserInfo>> GetUsers()
        {
            
                var usersinfo = from users in _context.Users
                                where users.IsDeleted == false
                                select new UserInfo()
                                {
                                    Id = users.Id,
                                    UserName = users.UserName,
                                    Email = users.Email
                                };
                List<UserInfo> userInfos = await usersinfo.ToListAsync<UserInfo>();
                return userInfos;
            
        }
        [HttpGet("{id}")]
        public async Task<UserInfo> GetUser(Guid id)
        {

            var usersinfo = from users in _context.Users
                            where users.IsDeleted == false
                            && users.Id == id
                            select new UserInfo()
                            {
                                Id = users.Id,
                                UserName = users.UserName,
                                Email = users.Email
                            };
            List<UserInfo> userInfos = await usersinfo.ToListAsync<UserInfo>();
            UserInfo user = userInfos.SingleOrDefault();

            return user;

        }


        [HttpGet("{id}/postscomments")]
        [ProducesResponseType(typeof(UserPostsCommentsInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPostsAndCommentsByUserId(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => id == u.Id && u.IsDeleted == false);

            var usertester = _context.Users.Include(x => x.Posts)
                                           .ThenInclude(x => x.Comments)
                                           .Single(x => x.Id == id &&x.IsDeleted == false);
            var usercomments = _context.Users.Include(x => x.Comments)
                                                .Single(x => x.Id == id && x.IsDeleted == false);
            UserPostsCommentsInfo userpostinfo = new UserPostsCommentsInfo()
            {
                UserName = usertester.UserName,
                Email = usertester.Email,
                Id = usertester.Id,
                
            };
            foreach(var post in usertester.Posts)
            {
                if (post.IsDeleted == false)
                {
                    PostInfo postInfo = new PostInfo()
                    {
                        Id = post.Id,
                        Created = post.Created,
                        Author = post.Author,
                        Body = post.Description,
                        Title = post.Title,
                        Updated = post.Updated, 
                        UserId=post.UserId
                    };
                    userpostinfo.Posts.Add(postInfo);
                }
            }
            userpostinfo.Posts = userpostinfo.Posts.OrderBy(p => p.Updated).ToList<PostInfo>();
            userpostinfo.Posts.Reverse();
            foreach (var comment in usertester.Comments)
            {
                if (comment.IsDeleted == false)
                {
                    CommentInfo commentInfo = new CommentInfo()
                    {
                        Id = comment.Id,
                        Created = comment.Created,
                        Author = comment.Author,
                        Body = comment.CommentBody,

                        Updated = comment.Updated
                        ,UserId =comment.UserId
                        ,PostId = comment.PostId

                    };

                    userpostinfo.Comments.Add(commentInfo);
                }
            }
            userpostinfo.Comments = userpostinfo.Comments.OrderBy(p => p.Updated).ToList<CommentInfo>();
            userpostinfo.Comments.Reverse();
            return user == null ? NotFound() : Ok(userpostinfo);

        }
        //[HttpGet("{id}/postscomments")]
        //[ProducesResponseType(typeof(UserPostsInfo), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> GetPostsAndCommentsByUserId(Guid id)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(u=> id==u.Id &&u.IsDeleted==false);

        //    var usertester = _context.Users.Include(x => x.Posts).ThenInclude(x=>x.Comments).Single(x=> x.Id == id);
        //    UserPostsInfo userpostinfo = new UserPostsInfo()
        //    {
        //        UserName = usertester.UserName,
        //        Email = usertester.Email,
        //        Id  = usertester.Id,
        //        Posts = usertester.Posts,
        //        Comments = usertester.Comments
        //    };
        //    return user == null ? NotFound() : Ok(userpostinfo);

        //}


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
            //ResultCode code =
            var usernameExists = await _context.Users.FirstOrDefaultAsync(u =>  u.UserName == username &&u.IsDeleted==false);
            if (usernameExists == null)
            {
                
                if(SendCode(username, email, "You've made an account on TheForestMan! ")==ResultCode.ValidAdress)
                {
                    return Ok(username);
                }
                else
                {
                    return BadRequest("Eroare");
                }
            }
            else
            {
                return BadRequest("Already exists");
            }

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
                
                if(diff.TotalSeconds>60)
                {
                    return BadRequest("Too late");
                }
                else if(code.Code.Equals(userCode.Code.Code))
                {
                    HashHelper hashHelper = new HashHelper();
                    string hashedPassword = hashHelper.GetHash(userCode.Password);
                    User user = new User()
                    {
                        UserName = userCode.UserName,
                        Email = userCode.Email,
                        Password = hashedPassword,
                        Id = userCode.Id
                    };
                    await _context.Users.AddAsync(user);

                    await _context.SaveChangesAsync();
                    return await BuildToken(user);
                }
                else
                {
                    return BadRequest("Code not valid");
                }
                
            }
            else
            {
                return BadRequest("Eroare");
            }

        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("newpasscode")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNewPassCode([FromQuery] string username, [FromQuery] string email)
        {
            var userExists = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username && u.IsDeleted == false && u.Email == email);
            if (userExists != null)
            {
                var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
                var emailclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email));

                if (userclaim != null)
                {

                    if (!userclaim.Value.Equals(userExists.UserName) || !emailclaim.Value.Equals(userExists.Email))
                        return BadRequest("Not his account");
                }
                if (SendCode(username, email, "You want to change your password! ") == ResultCode.ValidAdress)
                {
                    return Ok(username);
                }
                else
                {
                    return BadRequest("Eroare");
                }
            }
            else
            {
                return BadRequest("User not found");
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] UserCode userCode)
        {
            var userToUpdate = await _context.Users.FirstOrDefaultAsync(u =>
                (u.UserName == userCode.UserName &&  u.Email == userCode.Email) && u.IsDeleted == false);
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
                _context.Entry(userToUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                UserInfo userInfo = new UserInfo()
                {
                    Id = userToUpdate.Id,
                    Email = userToUpdate.Email,
                    UserName = userToUpdate.UserName
                };
                return Ok(userInfo);
            }
            //var userToUpdate = user;
            return NoContent();
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("deletecode")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDeleteCode([FromQuery] string username, [FromQuery] string email)
        {
            var userExists = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username &&u.IsDeleted==false &&u.Email == email);
            if (userExists != null)
            {
                var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
                var emailclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Email));

                if (userclaim != null)
                {

                    if (!userclaim.Value.Equals(userExists.UserName) || !emailclaim.Value.Equals(userExists.Email))
                        return BadRequest("Not his account");
                }
                if (SendCode(username, email, "You want to delete your account! ") == ResultCode.ValidAdress)
                {
                    return Ok(username);
                }
                else
                {
                    return BadRequest("Eroare");
                }
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
            var userToDelete = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username &&u.IsDeleted==false);
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

                        var usertester = _context.Users.Include(x => x.Posts).ThenInclude(x => x.Comments).Single(x => x.Id == userToDelete.Id && x.IsDeleted == false) ;
                        var usercomments = _context.Users.Include(x=>x.Comments).Single(x => x.Id == userToDelete.Id && x.IsDeleted == false);
                        //usertester.Posts.Find(p => p.UserId == usertester.Id).Author = "[User Deleted]";
                        //_context.Entry(usertester.Posts).State = EntityState.Modified;
                        usertester.Posts.RemoveAll(p => p.IsDeleted == true);
                        usertester.Comments.RemoveAll(c => c.IsDeleted == true);
                        if (usertester.Posts != null)
                        {
                            for (int i = 0; i < usertester.Posts.Count; ++i)
                            {
                                var post = usertester.Posts[i];
                                post.Author = "[User Deleted]";
                                //post.UserId = null;
                                post.Updated = DateTime.Now;
                                //--i;
                                _context.Entry(post).State = EntityState.Modified;
                            }
                        }
                        if (usertester.Comments != null)
                        {
                            for (int i = 0; i < usertester.Comments.Count; ++i)
                            {
                                var comment = usertester.Comments[i];
                                comment.Author = "[User Deleted]";
                                //comment.UserId = null;
                                comment.Updated = DateTime.Now;
                                _context.Entry(comment).State = EntityState.Modified;
                            }
                        }
                        userToDelete.IsDeleted = true;
                        _context.Entry(userToDelete).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        return NoContent();
                    }
                    else
                    {
                        return BadRequest("Code not valid");
                    }
                }
            }
            return BadRequest("Eroare");
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] Credentials credentials)
        {
            //fa doar cu username sau doar cu email
            HashHelper hashHelper = new HashHelper();
            string hashedPassword = hashHelper.GetHash(credentials.Password);
            var user = _context.Users.SingleOrDefault(x=> x.IsDeleted == false 
            && x.Password == hashedPassword
            &&(x.Email==credentials.NameEmail|| x.UserName == credentials.NameEmail));
            if (user == null) return BadRequest("Invalid login attempt");
            else return await BuildToken(user);
            


        }
        
        private async Task<UserToken> BuildToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
                //new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };




            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTkey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.Now.AddDays(1);

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
    }
}
