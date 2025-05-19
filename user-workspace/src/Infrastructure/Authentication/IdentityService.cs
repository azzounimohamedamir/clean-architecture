using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

public class IdentityService : IIdentityService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public IdentityService(ApplicationDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<(bool Success, string UserId)> CreateUserAsync(string userName, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Username == userName))
        {
            return (false, string.Empty);
        }

        var user = new ApplicationUser
        {
            Username = userName,
            PasswordHash = HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return (true, user.Id.ToString());
    }

    public async Task<bool> ValidateUserAsync(string userName, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);
        if (user == null)
        {
            return false;
        }

        return VerifyPassword(password, user.PasswordHash);
    }

    public async Task<string> GenerateJwtTokenAsync(string userName)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);
        if (user == null)
        {
            throw new ApplicationException("User not found");
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<(Result Result, string UserId)> DeleteUserAsync(string userId)
    {
        var user = await _context.Users.FindAsync(int.Parse(userId));

        if (user == null)
        {
            return (Result.Failure(new[] { "User not found" }), string.Empty);
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return (Result.Success(), userId);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}
