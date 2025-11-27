using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<AuthResponse?> Authenticate(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return null;
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;

            var token = GenerateJwt(user);
            return new AuthResponse
            {
                Token = token,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role.ToString()
            };
        }

        public async Task<UserDto> Register(string name, string email, string password)
        {
            if (await _db.Users.AnyAsync(u => u.Email == email))
                throw new Exception("Email exists");

            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = Role.User
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return new UserDto { Id = user.Id, Name = user.Name, Email = user.Email, Role = user.Role.ToString() };
        }

        private string GenerateJwt(User user)
        {
            var jwt = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpiresMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}