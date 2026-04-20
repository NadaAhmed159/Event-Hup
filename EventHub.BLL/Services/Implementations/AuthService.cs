using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EventHub.BLL.Configuration;
using EventHub.BLL.Models;
using EventHub.BLL.Services.Interfaces;
using EventHub.DAL.Repositories.Interfaces;
using EventHub.Domain.Entities;
using EventHub.Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EventHub.BLL.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtOptions _jwt;

        public AuthService(IUnitOfWork unitOfWork, IOptions<JwtOptions> jwtOptions)
        {
            _unitOfWork = unitOfWork;
            _jwt = jwtOptions.Value;
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Email and password are required.");

            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
                throw new InvalidOperationException("Invalid email or password.");

            if (!VerifyPassword(request.Password, user.Password))
                throw new InvalidOperationException("Invalid email or password.");

            return CreateAuthResult(user);
        }

        public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Email and password are required.");
            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                throw new ArgumentException("First name and last name are required.");

            var email = request.Email.Trim().ToLowerInvariant();
            if (await _unitOfWork.Users.EmailExistsAsync(email))
                throw new InvalidOperationException("Email already exists.");

            var user = new User
            {
                Email = email,
                Password = HashPassword(request.Password),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                ApplyAs = request.ApplyAs,
                PhoneNumber = request.PhoneNumber,
                Status = AccountStatus.Pending
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return CreateAuthResult(user);
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.NewPassword))
                throw new ArgumentException("Email and new password are required.");

            var email = request.Email.Trim().ToLowerInvariant();
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            user.Password = HashPassword(request.NewPassword);
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        private AuthResult CreateAuthResult(User user)
        {
            var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpiresMinutes <= 0 ? 60 : _jwt.ExpiresMinutes);
            var token = CreateJwtToken(user, expires);
            return new AuthResult
            {
                Token = token,
                ExpiresAtUtc = expires,
                User = MapUser(user)
            };
        }

        private static AuthUserDto MapUser(User user) =>
            new()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ApplyAs = user.ApplyAs,
                PhoneNumber = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl
            };

        private string CreateJwtToken(User user, DateTime expiresUtc)
        {
            if (string.IsNullOrWhiteSpace(_jwt.Key) || _jwt.Key.Length < 32)
                throw new InvalidOperationException("Jwt:Key must be configured and at least 32 characters.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.ApplyAs.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expiresUtc,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string HashPassword(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        private static bool VerifyPassword(string plain, string stored)
        {
            if (string.IsNullOrEmpty(stored))
                return false;
            if (stored.StartsWith("$2", StringComparison.Ordinal))
                return BCrypt.Net.BCrypt.Verify(plain, stored);
            return plain == stored;
        }
    }
}
