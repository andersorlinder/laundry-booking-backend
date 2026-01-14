using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using laundry_booking_backend.Data;
using laundry_booking_backend.Dtos;
using laundry_booking_backend.Models;

namespace laundry_booking_backend.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse?> RegisterAsync(RegisterRequest request);
    string GenerateToken(User user);
    bool ValidatePin(string pin);
}

public class AuthService : IAuthService
{
    private readonly LaundryDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(LaundryDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> RegisterAsync(RegisterRequest request)
    {
        // Validate PIN format
        if (!ValidatePin(request.Pin))
        {
            return null; // Invalid PIN format
        }

        // Check if email already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
        {
            return null; // User already exists
        }

        var user = new User
        {
            Email = request.Email,
            PasswordPin = HashPin(request.Pin),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Token = GenerateToken(user)
        };
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return null; // User not found
        }

        if (!VerifyPin(request.Pin, user.PasswordPin))
        {
            return null; // Invalid PIN
        }

        return new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Token = GenerateToken(user)
        };
    }

    public string GenerateToken(User user)
    {
        var jwtSecret = _configuration["JwtSettings:Secret"];
        var jwtIssuer = _configuration["JwtSettings:Issuer"];
        var jwtAudience = _configuration["JwtSettings:Audience"];

        if (string.IsNullOrEmpty(jwtSecret) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
        {
            throw new InvalidOperationException("JWT settings are not configured properly");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidatePin(string pin)
    {
        // PIN must be exactly 4 digits
        return !string.IsNullOrWhiteSpace(pin) && pin.Length == 4 && pin.All(char.IsDigit);
    }

    private string HashPin(string pin)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pin));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private bool VerifyPin(string pin, string hash)
    {
        var hashOfInput = HashPin(pin);
        return hashOfInput == hash;
    }
}
