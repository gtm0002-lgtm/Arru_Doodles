using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API_Doodles_2._0.Data;
using API_Doodles_2._0.Dto;
using API_Doodles_2._0.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace API_Doodles_2._0.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IPasswordHasher<Users> _hasher;
    private readonly IConfiguration _config;

    public AuthController(DatabaseContext context, IPasswordHasher<Users> hasher, IConfiguration config)
    {
        _context = context;
        _hasher = hasher;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LogIn([FromBody] LogInDto dto)
    {
        // Ask the database for the introduced email:
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == dto.Email);
        // If the email is not found, return Unauthorized:
        if (user == null) return Unauthorized(new { error = "Invalid Email" });

        // Check if the password is correct:
        var hasher = _hasher.VerifyHashedPassword(user, user.Password, dto.Password);
        // If it isn't, return Unauthorized:
        if (hasher == PasswordVerificationResult.Failed) return Unauthorized(new { error = "Invalid Password" });

        // If all is correct, generate the JWT token associating it into a User and return it:
        var token = GenerateJwtToken(user);
        var userDto = new UserDto()
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName
        };

        return Ok(new { token, user = userDto });
    }

    [HttpGet]
    [Authorize]
    public IActionResult ValidToken()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var email = User.FindFirstValue(ClaimTypes.Email);

        return Ok(new { valid = true, id, email });
    }

    private string GenerateJwtToken(Users user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? string.Empty));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _config["Issuer"],
            audience: _config["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}