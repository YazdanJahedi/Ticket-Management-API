using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicketAPI.Models;

namespace TicketAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _conf;

        public AuthController(IConfiguration conf, ApplicationDbContext context)
        {
            _conf = conf;
            _context = context;
        }

        [HttpPost("signup")]
        public async Task<ActionResult<User>> Signup(UserDto req)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            if (IsUserFound(req.Email))
            {
                return BadRequest("this Email is used before");
            }

            string passHash = BCrypt.Net.BCrypt.HashPassword(req.Password);

            // first user is admin
            string role = "User";
            if (_context.Users.IsNullOrEmpty())
            {
                role = "Admin";
            }

            var user = new User
            {
                Name = req.Name,
                Email = req.Email,
                Role = role,
                PhoneNumber = req.PhoneNumber,
                PasswordHash = passHash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }


        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserLoginInfo req)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            if (!IsUserFound(req.Email))
            {
                return BadRequest("Email not found");
            }

            var user = _context.Users.FirstOrDefault(e => e.Email == req.Email);

            if (user == null)
            {
                return NotFound();
            }

            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                return BadRequest("password not correct");

            var token = CreateToken(user);

            return Ok(user + "\n\nbearer " + token);
        }


        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.Role!),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }


        [HttpGet("test"), Authorize]
        public string Test()
        {
            var email = User.Claims.Where(x => x.Type == ClaimTypes.Email).FirstOrDefault()?.Value;
            var role = User.Claims.Where(x => x.Type == ClaimTypes.Role).FirstOrDefault()?.Value;
            var id = User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;
            return email + " " + role + " " + id;
        }


        private bool IsUserFound(string email)
        {
            return (_context.Users?.Any(e => e.Email == email)).GetValueOrDefault();
        }

    }
}
