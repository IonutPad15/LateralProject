using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using API.Utils;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly TFMDbContext _context;
        public UserController(TFMDbContext context)
        {
            _context = context;
        }

        //inca nu sunt sigur daca am nevoie de lista de useri
        //dar o las momentat
        [HttpGet]
        public async Task<IEnumerable<UserModel>> Get()
        {
            return await _context.Users.ToListAsync();
        }


        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByid(int id)
        {
            var User = await _context.Users.FindAsync(id);
            return User == null ? NotFound() : Ok(User);

        }


        [HttpPost]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status302Found)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(UserModel User)
        {
            //cauta daca exista deja un utilizator cu acel username
            var matchesUser = from users in _context.Users
                          where users.UserName == User.UserName
                          select users;
            //cauta daca exita deja un utilizator cu acel email
            var matchesEmail = from users in _context.Users
                               where users.Email == User.Email
                               select users;
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
            }
            //daca totul e bine, trimite un email cu codul de siguranta si salveaza userul in db
            //in MVC voi verifica codul de siguranta?
            Sender sender = new Sender();
            string verif = sender.sendEmail(User.Email);
            if(!verif.Equals("") && !verif.Equals("err1"))
                await _context.Users.AddAsync(User);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetByid), new { id = User.Id }, User);
        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, UserModel User)
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
            var UserToDelete = await _context.Users.FindAsync(id);
            if (UserToDelete == null) return NotFound();

            _context.Users.Remove(UserToDelete);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
