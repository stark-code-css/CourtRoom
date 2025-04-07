using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CourtRoom.Configurations;
using CourtRoom.Data;
using CourtRoom.Dtos.AuthDtos;
using CourtRoom.Models;
using CourtRoom.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CourtRoom.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(AppDbContext context, IAuthService authService, IOptions<JwtSettings> jwtOptions)
    : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly IAuthService _authService = authService;
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterAppUserDto dto)
    {
        try
        {
            var hashedPassword = _authService.HashPassword(dto.Email, dto.Password);

            AppUser appUser = new()
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = hashedPassword,
                Role = dto.Role
            };

            _context.AppUsers.Add(appUser);
            await _context.SaveChangesAsync();
            return Ok("User registered");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginAppUserDto dto)
    {
        try
        {
            var existingUser = await _context.AppUsers.FirstOrDefaultAsync(a => a.Email == dto.Email);
            if (existingUser == null) return Unauthorized("Invalid email or password");

            var isCorrectPassword = _authService.VerifyHashedPassword(dto.Email, dto.Password, existingUser.Password);
            if (!isCorrectPassword) return Unauthorized("Invalid email or password");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, existingUser.Name),
                new Claim(ClaimTypes.Email, existingUser.Email),
                new Claim(ClaimTypes.Role, existingUser.Role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: credentials
            );


            return Ok(new
            {
                name = existingUser.Name,
                email = existingUser.Email,
                token = new JwtSecurityTokenHandler().WriteToken(token),
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}