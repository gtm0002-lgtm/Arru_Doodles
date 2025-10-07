using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_Doodles_2._0.Data;
using API_Doodles_2._0.Models;
using API_Doodles_2._0.Dto;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography.X509Certificates;

namespace API_Doodles_2._0.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly DatabaseContext _context;
    private readonly IPasswordHasher<Users> _hasher;

    public UsersController(DatabaseContext context, IPasswordHasher<Users> hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var users = await _context.Users
                .Select(u => new UserDto { Id = u.Id, Email = u.Email, UserName = u.UserName })
                .ToListAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }
    [HttpPost]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return Conflict(new { error = "Email Already Exists" });

        var user = new Users { UserName = dto.UserName, Email = dto.Email };
        user.Password = _hasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = new UserDto { Email = user.Email, UserName = user.UserName, Id = user.Id };
        return CreatedAtAction(nameof(GetById), new {id = user.Id}, result);
    }

    [HttpGet("{id}")]

    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();
        
        return Ok(new UserDto { Email = user.Email, UserName = user.UserName, Id = user.Id });
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] RegisterDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found" });

        // Actualizamos solo los campos necesarios
        user.UserName = dto.UserName ?? user.UserName;
        user.Email = dto.Email ?? user.Email;

        // Solo si envías un nuevo password
        if (!string.IsNullOrEmpty(dto.Password))
            user.Password = _hasher.HashPassword(user, dto.Password);

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { message = "User updated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new { message = "User not found" });

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User deleted successfully" });
    }

}