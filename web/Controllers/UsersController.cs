using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using web.function;
using web.models;
using web.Models;

namespace web.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly userContext _context;
        private readonly JWTSettings _jwtsettings;

        public UsersController(userContext context,IOptions<JWTSettings> jwtsettings)
        {
            _context = context;
            _jwtsettings = jwtsettings.Value;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        
        //перевірка логіна пароля 
        [HttpGet("Login")]
        public async Task<ActionResult<UserWithToken>> Login ([FromBody]Users user)
        {
            
            var u = await _context.Users.Where(u => u.Login == user.Login && u.Parol == user.Parol)
                .FirstOrDefaultAsync();

            UserWithToken userWithToken = new UserWithToken(u);

            if (userWithToken == null)
            {
                return NotFound();
            }
            Log log = new Log();
            log.IdUsers = u.Id;
            log.IdEvent = 1;
            log.Date= DateTime.UtcNow;
            

            _context.Log.Add(log);
           
            _context.SaveChanges();

            //token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Login),
                }),
                Expires = DateTime.UtcNow.AddMonths(1),// час видалення токену 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            userWithToken.Token = tokenHandler.WriteToken(token);
      
            return userWithToken;
        }
       

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsers(int id, Users users)
        {
            if (id != users.Id)
            {
                return BadRequest();
            }

            _context.Entry(users).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Users>> PostUsers(Users users)
        {

            _context.Users.Add(users);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsers", new { id = users.Id }, users);
        }
        [HttpPut("resetpassword")]
        public async Task<ActionResult<Users>> ResetPassword(Users user)
        {
            var users = _context.Users.Where(u => u.Login == user.Login).FirstOrDefault();
            if(users == null)
            { NotFound(); }
            else {
                users.Parol= user.Parol;
                _context.SaveChanges();
                Log log = new Log();
                log.IdUsers = users.Id;
                log.IdEvent = 2;
                log.Date = DateTime.UtcNow;


                _context.Log.Add(log);

                _context.SaveChanges();
            }  
            
          return users;
        }
        //[HttpGet("/log")]
        //public async Task<ActionResult<Log>> ()
        //{
        //    var log = new Log();

        //    var Logs = await _context.Log.Include(l => log.Id).Where(l => log.IdUsers == 0).FirstOrDefaultAsync();
        //    if (Logs == null)
        //    {
        //        return NotFound();
        //    }

        //    return Logs;
        //}

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Users>> DeleteUsers(int id)
        {
            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            _context.Users.Remove(users);
            await _context.SaveChangesAsync();

            return users;
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
