using ASP_NET.Data;
using ASP_NET.DTOs.User;
using ASP_NET.Enum;
using ASP_NET.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;

    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDTO model)
    {
        if (_context.Users.Any(u => u.Email == model.Email))
            return BadRequest("Email already exists");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
        var user = new User
        {
            Email = model.Email,
            PasswordHash = hashedPassword,
            Role = UserRole.USER
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDTO model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        var token = GenerateJwtToken(user);

        return Ok(new
        {
            message = "Login success",
            token,
            role = user.Role.ToString()
        });
    }
    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role.ToString()) // sử dụng Enum
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    [Authorize(Roles = "ADMIN")]
    [HttpGet("admin-only")]
    public IActionResult AdminEndpoint()
    {
        return Ok("Welcome, admin!");
    }

    [Authorize(Roles = "USER")]
    [HttpGet("user-only")]
    public IActionResult UserEndpoint()
    {
        return Ok("Welcome, user!");
    }
}

