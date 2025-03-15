using Application.Interfaces;
using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
            {
                return BadRequest(new { message = "Invalid request. Name, Email, and Password are required." });
            }

            var existingUser = await _userService.GetUserByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Email is already in use." });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            await _userService.AddUserAsync(user);

            return Ok(new { message = "User registered successfully!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            if (login == null || string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.PasswordHash))
            {
                return BadRequest(new { message = "Invalid login request." });
            }

            var user = await _userService.GetUserByEmailAsync(login.Email);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (!BCrypt.Net.BCrypt.Verify(login.PasswordHash, user.PasswordHash)) 
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            // Generate JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(1), 
                Issuer = _configuration["Jwt:Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { token = tokenString });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { message = "Unauthorized request." });
            }

            var user = await _userService.GetUserByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(new
            {
                user.Name,
                user.LastName,
                user.City,
                user.Country,
                user.State,
                user.Email,
                user.Username
            });
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updatedUser)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { message = "Unauthorized request." });
            }

            var existingUser = await _userService.GetUserByEmailAsync(userEmail);

            if (existingUser == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Updating fields
            existingUser.Name = updatedUser.Name ?? existingUser.Name;
            existingUser.LastName = updatedUser.LastName ?? existingUser.LastName;
            existingUser.City = updatedUser.City ?? existingUser.City;
            existingUser.Country = updatedUser.Country ?? existingUser.Country;
            existingUser.State = updatedUser.State ?? existingUser.State;

            await _userService.UpdateUserAsync(existingUser);
            return Ok(new { message = "Profile updated successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(new { users });
        }
    }
}
