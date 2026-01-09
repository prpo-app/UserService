using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using UserService.Models;

namespace UserService.Controllers
{
    [Route("/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _config;
        private AppDbContext _context;

        public UserController(ILogger<UserController> logger, IConfiguration config, AppDbContext context)
        {
            _logger = logger;
            _config = config;
            _context = context;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="reg">User registration data (username and password).</param>
        /// <response code="200">User Created.</response>
        /// <response code="400">Username already exists.</response>
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUser reg)
        {
            if (string.IsNullOrWhiteSpace(reg.Username) || string.IsNullOrWhiteSpace(reg.Password))
                return BadRequest("Username and password required.");

            if (_context.Users.Any(u => u.Username == reg.Username))
                return BadRequest("Username already exists.");

            var hash = BCrypt.Net.BCrypt.HashPassword(reg.Password);

            var user = new User
            {
                Username = reg.Username,
                PasswordHash = hash
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="log">Login data (username and password).</param>
        /// <response code="200">User loged in.</response>
        /// <response code="400">Missing username or password.</response>
        /// <response code="401">Invalid username or password.</response>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginUser log)
        {
            if (string.IsNullOrWhiteSpace(log.Username) || string.IsNullOrWhiteSpace(log.Password))
                return BadRequest("Username and password required.");

            var user = _context.Users.SingleOrDefault(u => u.Username == log.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(log.Password, user.PasswordHash))
                return Unauthorized("Invalid username or password.");

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var jwt = _config.GetSection("Jwt");

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(jwt["ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}