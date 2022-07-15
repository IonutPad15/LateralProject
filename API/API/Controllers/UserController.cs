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
        [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByid(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            var userinfo = new UserInfo()
            {
                UserName = user.UserName,
                Email = user.Email,
                Id = user.Id
            };
            return user == null ? NotFound() : Ok(userinfo);

        }
        [HttpGet("{username}")]
        [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByUsername(  string username)
        {
            var user = await _context.Users.FindAsync(username);
            var userinfo = new UserInfo()
            {
                UserName = user.UserName,
                Email = user.Email,
                Id = user.Id
            };
            return user == null ? NotFound() : Ok(userinfo);

        }

        private enum ResultCode
        {
            InvalidAdress, ValidAdress, Error
        }
        [HttpPost("getcode")]
        public async Task<ActionResult> GetCode( User user)
        {

            //ResultCode code =
            Sender sender = new Sender();
            string verif = sender.SendEmail(user.Email);
            if (!verif.Equals(ResultCode.Error.ToString()) && !verif.Equals(ResultCode.InvalidAdress.ToString()))
            {
                RegisterCode code = new RegisterCode()
                {
                    Code = verif,
                    Created = DateTime.Now
                };
                userCodes.Add(user.UserName, code);
                return Ok(user);
            }
            else
            {
                return BadRequest("Eroare");
            }

        }
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
                else
                {
                    User user = new User()
                    {
                        UserName = userCode.UserName,
                        Email = userCode.Email,
                        Password = userCode.Password,
                        Id = userCode.Id
                    };
                    var result = await _context.Users.AddAsync(user);

                    await _context.SaveChangesAsync();
                    return await BuildToken(user);
                }
                
            }
            else
            {
                return BadRequest("Eroare");
            }

        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, User user)
        {
            var userclaim = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name));
            if (userclaim != null)
            {
                if (!userclaim.Value.Equals(user.UserName))
                    return BadRequest("Not his post");
            }
            
            if (id != user.Id) return BadRequest("Not good");
            _context.Entry(User).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            //TODO: variables start with a lowercase
            var userToDelete = await _context.Users.FindAsync(id);
            if (userToDelete == null) return NotFound();

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] User user)
        {

            var foundUser = from users in _context.Users
                            where (users.Email == user.Email
                            || users.UserName == user.UserName)
                            && users.Password == user.Password
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
