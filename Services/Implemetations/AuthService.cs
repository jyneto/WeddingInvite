using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeddingInvite.Api.Data;
using WeddingInvite.Api.DTOs.AuthDTO;
using WeddingInvite.Api.Models;
using WeddingInvite.Api.Repositories.Interfaces;
using WeddingInvite.Api.Services.Interfaces;

namespace WeddingInvite.Api.Services.Implemetations
{
    public class AuthService : IAuthService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IConfiguration _configuration;
        public AuthService(IAdminRepository adminRepository, IConfiguration configuration)
        {
            _adminRepository = adminRepository;
            _configuration = configuration;
        }

        public async Task<bool> RegisterAsync(AdminRegisterDTO adminRegisterDTO)
        {
            var usernameTaken = await _adminRepository.UserNameExistsAsync(adminRegisterDTO.UserName);
            if (usernameTaken)
                return false;
            var hashedPassword = await HashPassword(adminRegisterDTO.Password!);
            var newAdmin = new Admin
            {
                UserName = adminRegisterDTO.UserName,
                PasswordHash = hashedPassword
            };
            return await _adminRepository.AddAdminAsync(newAdmin);
        }
        public async Task<AdminResponseDTO?> LoginAsync(AdminLoginDTO adminLoginDTO)
        {
            var admin = await _adminRepository.GetByUserNameAsync( adminLoginDTO.Username!);
            if (admin == null || !BCrypt.Net.BCrypt.Verify(adminLoginDTO.Password!, admin.PasswordHash!))
                return null;

            var token = GenerateJwtToken(admin);
            var response = new AdminResponseDTO
            {
                Username = admin.UserName,
                Token = token
            };
            return response;
        }
        public Task<string> HashPassword(string password)
        {
            return Task.FromResult(BCrypt.Net.BCrypt.HashPassword(password));
        }

        public Task<bool> VerifyPassword(string password, string hash)
        {
            return Task.FromResult(BCrypt.Net.BCrypt.Verify(password, hash));
        }
        private string GenerateJwtToken(Admin admin)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                new Claim(ClaimTypes.Name, admin.UserName),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, admin.UserName),
                    new Claim(ClaimTypes.Role, "Admin")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
