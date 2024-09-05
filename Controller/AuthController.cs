using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using CQRS_Microservice.Data;
using CQRS_Microservice.Models;
using Microsoft.AspNetCore.Identity;

namespace CQRS_Microservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthController(TokenService tokenService, ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _tokenService = tokenService;
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("generate-token")]
        public async Task<IActionResult> GenerateToken([FromBody] UserLoginModel model)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == model.Username);

            if (user == null)
                return Unauthorized("Invalid username or password.");

            // Verify the password
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Success)
            {
                var token = _tokenService.GenerateToken(user.Username, user.Permissions);
                return Ok(new { Token = token });
            }

            return Unauthorized("Invalid username or password.");
        }
    }

    public class UserLoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
