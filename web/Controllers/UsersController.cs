using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
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
    [Produces("application/json")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly userContext _context;
        private readonly JWTSettings _jwtsettings;


        public UsersController(userContext context, IOptions<JWTSettings> jwtsettings)
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
        [HttpPost("registration")]
        public async Task<ActionResult<UserWithToken>> Registration([FromBody] Users user)
        {
            var u = await _context.Users.Where(u => u.Login == user.Login).FirstOrDefaultAsync();
            if (u == null)
            {
                var userdb = new Users();
                UserWithToken userWithToken;
                try
                {
                    userdb.Login = user.Login;
                    userdb.Name = user.Name;
                    userdb.Surname = user.Surname;

                    string password = userdb.Parol;
                    using (MD5 md5 = MD5.Create())
                    {
                        byte[] hashedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(user.Parol));
                        string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                        userdb.Parol = hashedPassword;

                    }

                    _context.Users.Add(userdb);
                    _context.SaveChanges();

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
                    userWithToken = new UserWithToken(userdb);
                    userWithToken.Token = tokenHandler.WriteToken(token);

                    Log log = new Log();
                    log.IdUsers = userdb.Id;
                    log.IdEvent = 4;
                    log.Date = DateTime.UtcNow;

                    _context.Log.Add(log);

                    _context.SaveChanges();
                }
                catch
                {
                    return StatusCode(500, "Помилка реєстрації");
                }
                return userWithToken;
            }
            else { return StatusCode(500, "Такий логін вже існує"); }    
        }

            


        [HttpPost("Login")]
public async Task<ActionResult<UserWithToken>> Login([FromBody] Users user)
{
    string password = user.Parol;
    using (MD5 md5 = MD5.Create())
    {
        byte[] hashedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(user.Parol));
        string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

        user.Parol = hashedPassword;
    }

    var u = await _context.Users.Where(u => u.Login == user.Login && u.Parol == user.Parol)
        .FirstOrDefaultAsync();
    try
    {
        if (u == null)
        {
            return NotFound();
        }

        UserWithToken userWithToken = new UserWithToken(u);

        Log log = new Log();
        log.IdUsers = u.Id;
        log.IdEvent = 1;
        log.Date = DateTime.UtcNow;


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
    catch
    {

        return NotFound();

    }

}
[HttpGet("status")]
public bool status()
{
    return true;
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
    var users = _context.Users.Where(u => u.Id == user.Id).FirstOrDefault();
    if (users == null)
    {
        return NotFound();
    }

    else
    {
        users.Parol = user.Parol;

        // Хеширование пароля с использованием MD5
        using (MD5 md5 = MD5.Create())
        {
            byte[] hashedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(user.Parol));
            string hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            users.Parol = hashedPassword;
        }



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
[HttpPut("ChangeData")]
public async Task<ActionResult<Users>> ChangeData(Users user)
{
    var users = _context.Users.Where(u => u.Id == user.Id).FirstOrDefault();
    if (users == null)
    {
        return NotFound();
    }

    else
    {
        users.Name = user.Name;
        users.Surname = user.Surname;
        _context.SaveChanges();
        Log log = new Log();
        log.IdUsers = users.Id;
        log.IdEvent = 3;
        log.Date = DateTime.UtcNow;

        _context.Log.Add(log);

        _context.SaveChanges();
    }
    return users;
}

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
