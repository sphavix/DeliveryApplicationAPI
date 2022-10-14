using AuthenticationPlugin;
using DeliveryApp.Data;
using DeliveryApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DeliveryApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly AuthService _auth;

        public AccountsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _auth = new AuthService(_configuration);
        }
        //[HttpGet]
        //public IActionResult Register(User user)
        //{
        //    return Ok();
        //}

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(User user)
        {
            var emailAlreadyExist = _context.Users.SingleOrDefault(x => x.Email == user.Email);
            if(emailAlreadyExist != null)
            {
                return BadRequest("This email address already exist, please use a different email.");
            }

            var userObj = new User
            {
                Email = user.Email,
                Name = user.Name,
                Password = SecurePasswordHasherHelper.Hash(user.Password),
                Role = "Admin"
            };
            _context.Users.Add(userObj);
            await _context.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public IActionResult Login(User model)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == model.Email);
            if(user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            var password = user.Password;
            if(!SecurePasswordHasherHelper.Verify(model.Password, password))
            {
                return Unauthorized();
            }
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, model.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, model.Email),
                new Claim(ClaimTypes.Name, model.Email),
                new Claim(ClaimTypes.Role, user.Email)
            };

            var token = _auth.GenerateAccessToken(claims);
            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                toke_type = token.TokenType,
                user_Id = user.Id,
                user_name = user.Email,
                expires_in = token.ExpiresIn,
                creation_time = token.ValidFrom,
                expiration_time = token.ValidTo
            });
        }
    }
}
