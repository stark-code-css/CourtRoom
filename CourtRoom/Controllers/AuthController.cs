using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CourtRoom.Configurations;
using CourtRoom.Data;
using CourtRoom.Dtos.AuthDtos;
using CourtRoom.Models;
using CourtRoom.Services;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Register(RegisterAppUserDto dto)
    {
        try
        {
            var hashedPassword = _authService.HashPassword(dto.Email, dto.Password);

            if (dto.Role != "CourtMaster" && dto.Role != "Cashier")
            {
                return BadRequest(new
                {
                    success = false,
                    message = "You can only register a CourtMaster or a Cashier (case-sensitive)."
                });
            }

            AppUser appUser = new()
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = hashedPassword,
                Role = dto.Role
            };

            _context.AppUsers.Add(appUser);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "User registered successfully."
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
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
                role = existingUser.Role,
                token = new JwtSecurityTokenHandler().WriteToken(token),
            });
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserInfo()
    {
        try
        {
            var users = await _context.AppUsers.Select(u=>new {u.Id, u.Name, u.Email, u.Role}).ToListAsync();
            return Ok(new {
                success = true,
                message = "All users retrieved successfully.",
                data = users
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound("User Not Found");
            _context.AppUsers.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "User deleted successfully."
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPut("ResetPassword")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        try
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return NotFound(new
            {
                success = false,
                message = "User Not Found"
            });

            user.Password = _authService.HashPassword(dto.Email, "12345678");
            _context.AppUsers.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Password reset successfully."
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
    }

    [HttpPut("ChangePassword")]
    [Authorize(Roles = "Admin, Cashier, CourtMaster")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        try
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return NotFound(new
                {
                    success = false,
                    message = "User Not Found"
                });

            var isCorrectOldPassword = _authService.VerifyHashedPassword(dto.Email, dto.OldPassword, user.Password);
            if (!isCorrectOldPassword) return Unauthorized(new
            {
                success = false,
                message = "Old password does not match"
            });
            
            user.Password = _authService.HashPassword(dto.Email, dto.NewPassword);
            _context.AppUsers.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "Password changed successfully."
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                success = false,
                message = e.Message
            });
        }
    }
}
