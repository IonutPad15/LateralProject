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

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly SiteDbContext _context;
        private readonly IConfiguration _configuration;
        public UserController(SiteDbContext context, IConfiguration configuration)
        {
            _context = context;            _configuration = configuration;

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
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByid(int id)
        {
            var User = await _context.Users.FindAsync(id);
            return User == null ? NotFound() : Ok(User);

        }


        [HttpPost("create")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserToken>> Create([FromBody] User user)
        {
            //cauta daca exista deja un utilizator cu acel username
            /* var matchesUser = from users in _context.Users
                           where users.UserName == User.UserName
                           select users;
             //cauta daca exita deja un utilizator cu acel email
             var matchesEmail = from users in _context.Users
                                where users.Email == User.Email
                                select users;

             //TODO: you can adopt a Response<T> which contains the value and an enum for the error code
             if (matchesUser.Any())
             {
                 return StatusCode(StatusCodes.Status302Found);
                 //niste coduri care mi s-au parut ca s-ar potrivi
             }
             if (matchesEmail.Any())
             {
                 //am vrut sa fie diferite ca sa afiseze mesaje diferite utilizatorului
                 //"nume de utilizator existent" "exista deja un cont cu aceasta adresa"
                 return StatusCode(StatusCodes.Status409Conflict);
             }*/
            //daca totul e bine, trimite un email cu codul de siguranta si salveaza userul in db


            //TODO: no bussiness logic in the client. the logic there is just to render the api response
            

           
            Sender sender = new Sender();
            string verif = sender.SendEmail(user.Email);
            if (!verif.Equals("") && !verif.Equals("err1"))
            {
                var result = await _context.Users.AddAsync(user);
                    
                    await _context.SaveChangesAsync();
                    return await BuildToken(user);
                
            }
            else
            {
                return BadRequest("Eroare");
            }
           
        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid id, User User)
        {
            if (id != User.Id) return BadRequest();
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
