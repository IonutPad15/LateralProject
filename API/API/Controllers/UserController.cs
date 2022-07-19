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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static Dictionary<string, RegisterCode> userCodes= new Dictionary<string, RegisterCode>();
        private readonly SiteDbContext _context;
        private readonly IConfiguration _configuration;
        public UserController(SiteDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            //userCodes = new Dictionary<string, RegisterCode>();

        }

        //inca nu sunt sigur daca am nevoie de lista de useri
        //dar o las momentat
        [HttpGet]
        public async Task<IEnumerable<UserInfo>> Get()
        {
           
            var usersinfo = from users in _context.Users
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
        [ProducesResponseType(typeof(UserPostsInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByid(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            //var usertester = _context.Users.Include(x => x.Posts).ThenInclude(x => x.User).Single(x => x.Id == id);
            var usertester = _context.Users.Include(x => x.Posts).Single(x=> x.Id == id);
            UserPostsInfo userpostinfo = new UserPostsInfo()
            {
                UserName = usertester.UserName,
                Email = usertester.Email,
                Id  = usertester.Id,
                Posts = usertester.Posts
            };
            
            Console.WriteLine("\n\n\n\n");
            foreach (var post in usertester.Posts)
            {
                Console.WriteLine($"{usertester.UserName} has the post {post.Title} with body:{post.Description}");
            }
            Console.WriteLine("\n\n\n\n");
            var userinfo = new UserInfo()
            {
                UserName = user.UserName,
                Email = user.Email,
                Id = user.Id
            };
            return user == null ? NotFound() : Ok(userpostinfo);

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
                RegisterCode code = new RegisterCode()
                {
                    Code = verif,
                    Created = DateTime.Now
                };
                userCodes.Add(username, code);
                return ResultCode.ValidAdress;
            }
            
            if(verif.Equals(ResultCode.InvalidAdress.ToString()))   return ResultCode.InvalidAdress;
            
            return ResultCode.Error;
        }
        [HttpGet("registercode")]
        public async Task<ActionResult> GetRegisterCode([FromQuery] string username,[FromQuery] string email)
        {

            //ResultCode code =
            var usernameExists = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
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
        //[HttpPost("getcode")]
        //public async Task<ActionResult> GetCode( User user)
        //{

        //    //ResultCode code =
        //    Sender sender = new Sender();
        //    string verif = sender.SendEmail(user.Email);
        //    if (!verif.Equals(ResultCode.Error.ToString()) && !verif.Equals(ResultCode.InvalidAdress.ToString()))
        //    {
        //        RegisterCode code = new RegisterCode()
        //        {
        //            Code = verif,
        //            Created = DateTime.Now
        //        };
        //        userCodes.Add(user.UserName, code);
        //        return Ok(user);
        //    }
        //    else
        //    {
        //        return BadRequest("Eroare");
        //    }

        //}
        ///Sendemail
        [HttpPost("create")]
        [ProducesResponseType(typeof(UserCode), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserToken>> Create([FromBody] UserCode userCode)
        {

            if (userCodes.ContainsKey(userCode.UserName))
            {
                RegisterCode code = userCodes[userCode.UserName];
                DateTime dateTime = DateTime.Now;
                TimeSpan diff = dateTime.Subtract(code.Created);
                
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
                    var result = await _context.Users.AddAsync(user);

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
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(User user)
        {
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            
            
            if (userclaim != null)
            {
                if (!userclaim.Value.Equals(user.UserName))
                    return BadRequest("Not his account");
                var userToUpdate = await _context.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);

                HashHelper hashHelper = new HashHelper();
                string hashedPassword = hashHelper.GetHash(user.Password);


                userToUpdate.Password = hashedPassword;
                _context.Entry(userToUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(user);
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
            var usernameExists = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (usernameExists != null)
            {
               
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
        public async Task<IActionResult> Delete([FromQuery] string username, [FromQuery]string codeFromUser)
        {
            var userToDelete = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (userToDelete == null) return NotFound();
            //TODO: variables start with a lowercase
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            if (userclaim != null)
            {
                if (!userclaim.Value.Equals(userToDelete.UserName))
                    return BadRequest("Not his account");
                if (userCodes.ContainsKey(username))
                {
                    RegisterCode code = userCodes[username];
                    DateTime dateTime = DateTime.Now;
                    TimeSpan diff = dateTime.Subtract(code.Created);

                    if (diff.TotalSeconds > 60)
                    {
                        return BadRequest("Too late");
                    }
                    else if(code.Code.Equals(codeFromUser))
                    {
                        _context.Users.Remove(userToDelete);
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
        public async Task<ActionResult<UserToken>> Login([FromBody] User user)
        {
            HashHelper hashHelper = new HashHelper();
            string hashedPassword = hashHelper.GetHash(user.Password);
            var foundUser = from users in _context.Users
                            where (users.Email == user.Email
                            || users.UserName == user.UserName)
                            && users.Password == hashedPassword
                            select users;
            if(!foundUser.Any())
            {
                return BadRequest("Invalid login attempt");
            }
            else
            {
                return await BuildToken(user);
            }

            
        }
        private async Task<UserToken> BuildToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };




            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTkey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddDays(1);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpirationDate = expiration
            };

        }
    }
}
